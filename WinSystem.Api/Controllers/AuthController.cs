using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using WinSystem.Api.Data;
using WinSystem.Api.Models;
using WinSystem.Api.Services;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(AppDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public record LoginRequest(string Username, string Password);
        public record LoginResult(bool Success, string Message, UserDto? User, string? Token);

        public class UserDto
        {
            public string Id { get; set; } = "";
            public string Username { get; set; } = "";
            public string Name { get; set; } = "";
            public string? Position { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public int Status { get; set; }
            public string StatusText { get; set; } = "";
            public int LoginCount { get; set; }
            public DateTime? LastLoginAt { get; set; }
            public string? LastLoginIp { get; set; }
            public string? OrganizationId { get; set; }
            public List<string> Roles { get; set; } = new();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("login")]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest req)
        {
            var sw = Stopwatch.StartNew();

            // 登录失败统一出口：写审计日志（状态=失败，操作人取请求中的账号）后返回
            async Task<ActionResult<LoginResult>> Fail(string detail)
            {
                OperationLogHelper.Add(this, _db, "系统认证", "登录", detail, sw.ElapsedMilliseconds,
                    status: 2, username: req.Username);
                await _db.SaveChangesAsync();
                return Ok(new LoginResult(false, detail, null, null));
            }

            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return await Fail("请输入账号和密码。");

            var accountKey = $"u:{req.Username}";
            // 账号级锁定（防密码爆破）；IP 级由限流中间件兜底，避免共享 IP 被误伤
            if (LoginGuard.IsLocked(accountKey, out var until))
                return await Fail($"尝试次数过多，已临时锁定，请 {Math.Ceiling((until - DateTime.Now).TotalMinutes)} 分钟后再试。");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null)
            {
                LoginGuard.OnFailure(accountKey);
                return await Fail($"账号登录失败：{req.Username}（账号或密码错误）。");
            }
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                LoginGuard.OnFailure(accountKey);
                return await Fail($"账号登录失败：{req.Username}（账号或密码错误）。");
            }
            if (user.Status == 2)
                return await Fail($"账号登录失败：{user.Name}（该账号已被禁用）。");
            if (user.Status == 3)
                return await Fail($"账号登录失败：{user.Name}（该账号待激活）。");

            // 登录成功，清除失败记录
            LoginGuard.OnSuccess(accountKey);

            // 更新登录信息
            user.LastLoginAt = DateTime.Now;
            user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            user.LoginCount++;
            user.UpdatedAt = DateTime.Now;

            // 记录登录成功日志（操作人取请求中的账号，登录请求尚无 JWT；与登录信息一并保存）
            OperationLogHelper.Add(this, _db, "系统认证", "登录", $"用户登录：{user.Name}", sw.ElapsedMilliseconds,
                username: user.Username, name: user.Name);
            await _db.SaveChangesAsync();

            // 加载当前用户角色代码，写入 JWT，供服务端接口做角色授权（如 AdminOnly）
            var roleCodes = await (from ar in _db.AccountRoles
                                   join r in _db.Roles on ar.RoleId equals r.Id
                                   where ar.AccountId == user.Id
                                   select r.Code).ToListAsync();

            // 生成 JWT Token
            var token = GenerateJwtToken(user.Id, user.Username, user.Name, roleCodes, user.TokenVersion);

            return Ok(new LoginResult(true, "登录成功。", new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Position = user.Position,
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status,
                StatusText = user.StatusText,
                LoginCount = user.LoginCount,
                LastLoginAt = user.LastLoginAt,
                LastLoginIp = user.LastLoginIp,
                OrganizationId = user.OrganizationId,
                Roles = roleCodes
            }, token));
        }

        // 不接收客户端传入的账号 ID，身份一律取自 JWT，防止越权修改他人密码
        public record ChangePasswordRequest(string OldPassword, string NewPassword);
        public record ChangePasswordResult(bool Success, string Message);

        [HttpPost("change-password")]
        public async Task<ActionResult<ChangePasswordResult>> ChangePassword(ChangePasswordRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ChangePasswordResult(false, "登录状态无效，请重新登录。"));

            var user = await _db.Users.FindAsync(userId);
            if (user == null) return Unauthorized(new ChangePasswordResult(false, "账号不存在，请重新登录。"));
            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(req.OldPassword, user.PasswordHash))
                return Ok(new ChangePasswordResult(false, "当前密码错误。"));
            if (!Services.PasswordPolicy.IsValid(req.NewPassword))
                return Ok(new ChangePasswordResult(false, Services.PasswordPolicy.Error));
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            user.TokenVersion++;
            user.PasswordChangedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return Ok(new ChangePasswordResult(true, "密码修改成功。"));
        }

        /// <summary>生成 JWT Token（携带角色代码 Claim）。</summary>
        private string GenerateJwtToken(string userId, string username, string name, List<string> roleCodes, int tokenVersion = 0)
        {
            var jwtSection = _cfg.GetSection("Jwt");
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? jwtSection["Key"];
            if (string.IsNullOrEmpty(jwtKey)) throw new InvalidOperationException("缺少 JWT 密钥配置，请设置环境变量 JWT_KEY。");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId),
                new(JwtRegisteredClaimNames.UniqueName, username),
                new(ClaimTypes.Name, name),
                new("name", name),
                new("tv", tokenVersion.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var code in roleCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct())
                claims.Add(new Claim(ClaimTypes.Role, code));

            var expireHours = int.TryParse(jwtSection["ExpireHours"], out var h) ? h : 24;

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
