using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WinSystem.Api.Data;
using WinSystem.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 固定监听端口，供 WPF 客户端调用
builder.WebHost.UseUrls("http://localhost:5210");

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // 保持与 WPF 模型的 PascalCase 属性一致，避免大小写转换问题
        o.JsonSerializerOptions.PropertyNamingPolicy = null;
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MySQL 连接
var conn = builder.Configuration.GetConnectionString("MySql")
    ?? "Server=localhost;Port=3306;Database=winsystem;User=root;Password=root;";
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// JWT 认证：密钥优先取环境变量 JWT_KEY（避免硬编码提交到仓库），其次 appsettings 兜底
var jwtCfg = builder.Configuration.GetSection("Jwt");
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? jwtCfg["Key"]
    ?? throw new InvalidOperationException("缺少 JWT 密钥配置，请设置环境变量 JWT_KEY 或 appsettings Jwt:Key。");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtCfg["Issuer"],
            ValidAudience = jwtCfg["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        opt.Events = new JwtBearerEvents
        {
            // Token 吊销校验：比对 JWT 版本号与数据库账号 token_version，
            // 密码修改/重置或账号禁用后版本递增，旧 Token 立即失效。
            OnTokenValidated = async ctx =>
            {
                var sub = ctx.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sub))
                {
                    ctx.Fail("无有效的用户标识。");
                    return;
                }
                var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var user = await db.Users.FindAsync(sub);
                if (user == null || user.Status != 1)
                {
                    ctx.Fail("账号不存在或已禁用。");
                    return;
                }
                var tv = ctx.Principal!.FindFirstValue("tv");
                if (!int.TryParse(tv, out var tokenVer) || tokenVer != user.TokenVersion)
                {
                    ctx.Fail("登录凭证已失效，请重新登录。");
                }
            }
        };
    });

builder.Services.AddAuthorization(opt =>
{
    // 管理操作策略：仅 超级管理员 / 系统管理员 角色可访问
    opt.AddPolicy("AdminOnly", p => p.RequireRole("super_admin", "admin"));
});

// 登录接口限流：按来源 IP，每分钟最多 10 次，超限返回 429（防爆破兜底）
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

// 初始化数据库与种子数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// 操作日志：记录写操作，写入 sys_operation_log（登录与各业务模块的日志由对应控制器精确记录，避免重复/误判状态）
app.Use(async (ctx, next) =>
{
    var sw = Stopwatch.StartNew();
    var method = ctx.Request.Method;
    var path = ctx.Request.Path.Value ?? "";

    try
    {
        await next();
    }
    finally
    {
        sw.Stop();
        // 查询请求、登录、日志自身及已由控制器记录详细日志（含数据切片）的路径不在此记录，避免刷屏/重复
        var isControllerLogged = new[]
        {
            "/api/auth/login", "/api/operation-logs", "/api/schedules", "/api/users",
            "/api/organizations", "/api/menus", "/api/roles", "/api/configs",
            "/api/dict-types", "/api/messages"
        }.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        if (method != "GET" && !isControllerLogged)
        {
            try
            {
                var db = ctx.RequestServices.GetRequiredService<AppDbContext>();
                db.OperationLogs.Add(new SysOperationLog
                {
                    AccountId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Username = ctx.User.FindFirstValue(ClaimTypes.Name) ?? ctx.User.FindFirstValue(JwtRegisteredClaimNames.UniqueName),
                    Name = ctx.User.FindFirstValue("name") ?? ctx.User.Identity?.Name,
                    Module = OperationLogging.MapModule(path),
                    Action = OperationLogging.MapAction(method),
                    Method = method,
                    Path = path,
                    Detail = path,
                    Status = ctx.Response.StatusCode >= 200 && ctx.Response.StatusCode < 300 ? 1 : 2,
                    Ip = ctx.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = ctx.Request.Headers.UserAgent.ToString(),
                    DurationMs = sw.ElapsedMilliseconds,
                    CreatedAt = DateTime.Now
                });
                await db.SaveChangesAsync();
            }
            catch { /* 日志写入失败不影响主流程 */ }
        }
    }
});

app.MapControllers();

app.Run();

/// <summary>操作日志辅助：请求路径映射模块、HTTP 方法映射操作类型。</summary>
static class OperationLogging
{
    private static readonly (string Prefix, string Name)[] ModuleMap =
    {
        ("/api/users", "用户管理"),
        ("/api/organizations", "组织机构"),
        ("/api/menus", "菜单配置"),
        ("/api/roles", "角色权限"),
        ("/api/configs", "系统参数"),
        ("/api/dict-types", "数据字典"),
        ("/api/schedules", "日程管理"),
        ("/api/messages", "消息通知"),
        ("/api/auth", "登录认证"),
        ("/api/dashboard", "数据看板")
    };

    public static string MapModule(string path)
    {
        foreach (var (prefix, name) in ModuleMap)
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return name;
        return path;
    }

    public static string MapAction(string method) => method.ToUpperInvariant() switch
    {
        "POST" => "新增",
        "PUT" or "PATCH" => "编辑",
        "DELETE" => "删除",
        _ => method
    };
}
