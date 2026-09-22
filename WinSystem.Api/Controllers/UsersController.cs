using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;
using WinSystem.Api.Models;
using WinSystem.Api.Services;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // 列表（不返回密码哈希）
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var list = await _db.Users
                .OrderBy(u => u.Id)
                .Select(u => new
                {
                    u.Id, u.Username, u.Name, u.Avatar, u.Email, u.Phone, u.Position,
                    u.OrganizationId, u.LoginType, u.Status, StatusText = u.StatusText,
                    u.IsProtected, u.LastLoginAt, u.LastLoginIp, u.LoginCount, u.Remark,
                    u.CreatedAt, u.UpdatedAt
                })
                .ToListAsync();
            return Ok(list);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> Search([FromQuery] string? keyword, [FromQuery] int limit = 50)
        {
            var k = keyword?.Trim() ?? "";
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(k))
                query = query.Where(u => (u.Name ?? "").Contains(k) || (u.Username ?? "").Contains(k));
            var list = await query
                .OrderBy(u => u.Id)
                .Take(Math.Clamp(limit, 1, 200))
                .Select(u => new
                {
                    u.Id, u.Username, u.Name, u.OrganizationId,
                    OrganizationName = _db.Organizations.Where(o => o.Id == u.OrganizationId).Select(o => o.Name).FirstOrDefault() ?? ""
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(string id)
        {
            // 非管理员只能查看本人资料，防止枚举他人信息
            if (!User.IsInRole("super_admin") && !User.IsInRole("admin") &&
                id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(new
            {
                u.Id, u.Username, u.Name, u.Avatar, u.Email, u.Phone, u.Position,
                u.OrganizationId, u.LoginType, u.Status, StatusText = u.StatusText,
                u.IsProtected, u.LastLoginAt, u.LastLoginIp, u.LoginCount, u.Remark,
                u.CreatedAt, u.UpdatedAt
            });
        }

        public record UserInput(string? Username, string? Name, string? Password, string? Position,
            string? Email, string? Phone, string? OrganizationId, int? Status, int? LoginType, string? Remark,
            List<string>? RoleIds);

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult> Create(UserInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Name))
                return BadRequest("用户名和姓名不能为空。");
            if (!Services.PasswordPolicy.IsValid(input.Password))
                return BadRequest(Services.PasswordPolicy.Error);

            var exists = await _db.Users.AnyAsync(u => u.Username == input.Username);
            if (exists) return Conflict("该登录名已存在。");

            var Id = await NextAccountIdAsync();

            var user = new User
            {
                Id = Id,
                Username = input.Username.Trim(),
                Name = input.Name.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password.Trim()),
                Position = input.Position,
                Email = input.Email,
                Phone = input.Phone,
                OrganizationId = input.OrganizationId,
                Status = input.Status ?? 1,
                LoginType = input.LoginType ?? 1,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Users.Add(user);
            SaveRoles(Id, input.RoleIds);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "用户管理", "新增", $"新增用户：{user.Name}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(user));
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UserInput input)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(input.Password) && !Services.PasswordPolicy.IsValid(input.Password))
                return BadRequest(Services.PasswordPolicy.Error);

            // 受保护账号（如超级管理员）：禁止禁用、改登录名、改角色，防止误操作导致系统失管
            if (user.IsProtected == 1)
            {
                if (input.Status.HasValue && input.Status.Value != 1)
                    return BadRequest("受保护账号不允许被禁用。");
                if (!string.IsNullOrWhiteSpace(input.Username) && input.Username.Trim() != user.Username)
                    return BadRequest("受保护账号不允许修改登录名。");
                if (input.RoleIds != null)
                    return BadRequest("受保护账号不允许修改角色。");
            }

            var beforeJson = OperationLogHelper.Snapshot(user);
            var sw = Stopwatch.StartNew();
            if (!string.IsNullOrWhiteSpace(input.Username)) user.Username = input.Username.Trim();
            if (!string.IsNullOrWhiteSpace(input.Name)) user.Name = input.Name.Trim();
            if (!string.IsNullOrWhiteSpace(input.Password)) { user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password.Trim()); user.TokenVersion++; }
            if (input.Position != null) user.Position = input.Position;
            if (input.Email != null) user.Email = input.Email;
            if (input.Phone != null) user.Phone = input.Phone;
            if (input.OrganizationId != null) user.OrganizationId = input.OrganizationId;
            if (input.Status.HasValue && input.Status.Value != user.Status)
            {
                // 账号由正常变为锁定/禁用/待激活时，吊销其已签发的 Token
                if (input.Status.Value != 1) user.TokenVersion++;
                user.Status = input.Status.Value;
            }
            if (input.LoginType.HasValue) user.LoginType = input.LoginType.Value;
            if (input.Remark != null) user.Remark = input.Remark;
            user.UpdatedAt = DateTime.Now;

            if (input.RoleIds != null) SaveRoles(id, input.RoleIds);

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "用户管理", "编辑", $"编辑用户：{user.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(user));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>获取用户已分配的角色 ID 列表。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles(string id)
        {
            var roleIds = await _db.AccountRoles.Where(a => a.AccountId == id).Select(a => a.RoleId).ToListAsync();
            return Ok(roleIds);
        }

        /// <summary>获取用户所有角色拥有的菜单权限标识集合（用于侧边栏菜单过滤）。仅允许查询本人，管理员可查任意用户。</summary>
        [HttpGet("{id}/menu-permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetMenuPermissions(string id)
        {
            var currentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("super_admin") || User.IsInRole("admin");
            if (id != currentId && !isAdmin) return Forbid();
            var roleIds = await _db.AccountRoles.Where(a => a.AccountId == id).Select(a => a.RoleId).Distinct().ToListAsync();
            if (roleIds.Count == 0) return Ok(new List<string>());
            var keys = await _db.RoleMenus.Where(m => roleIds.Contains(m.RoleId)).Select(m => m.MenuKey).Distinct().ToListAsync();
            return Ok(keys);
        }

        /// <summary>更新账号角色关联（先删后插）。</summary>
        private void SaveRoles(string accountId, List<string>? roleIds)
        {
            _db.AccountRoles.RemoveRange(_db.AccountRoles.Where(a => a.AccountId == accountId));
            if (roleIds == null) return;
            var now = DateTime.Now;
            foreach (var roleId in roleIds.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
            {
                _db.AccountRoles.Add(new AccountRole { AccountId = accountId, RoleId = roleId.Trim(), CreatedAt = now });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.IsProtected == 1) return BadRequest("受保护账号不可删除。");
            var beforeJson = OperationLogHelper.Snapshot(user);
            var sw = Stopwatch.StartNew();
            _db.Users.Remove(user);
            _db.AccountRoles.RemoveRange(_db.AccountRoles.Where(a => a.AccountId == id));
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "用户管理", "删除", $"删除用户：{user.Name}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>重置密码为 123456。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.IsProtected == 1) return BadRequest("受保护账号不可重置密码，请用原密码在个人中心修改。");
            var beforeJson = OperationLogHelper.Snapshot(user);
            var sw = Stopwatch.StartNew();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Services.PasswordPolicy.DefaultResetPassword);
            user.TokenVersion++;
            user.PasswordChangedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "用户管理", "重置密码", $"重置用户密码:{user.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(user));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>生成下一个账号 ID：A10001、A10002…</summary>
        private async Task<string> NextAccountIdAsync()
        {
            var maxId = await _db.Users
                .Select(u => u.Id)
                .ToListAsync();
            var maxNum = maxId
                .Where(x => x.StartsWith("A") && int.TryParse(x[1..], out _))
                .Select(x => int.Parse(x[1..]))
                .DefaultIfEmpty(10000)
                .Max();
            return "A" + (maxNum + 1);
        }
    }
}