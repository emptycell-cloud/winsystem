using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using WinSystem.Api.Data;
using WinSystem.Api.Models;

namespace WinSystem.Api.Services
{
    /// <summary>操作日志辅助：供各业务控制器记录带修改前后数据切片的详细操作日志。</summary>
    public static class OperationLogHelper
    {
        private static readonly JsonSerializerOptions LogJsonOptions = new()
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // 快照数据直接保存中文，不做 \uXXXX 转义，便于日志阅读
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>将实体序列化为 JSON 快照字符串（中文不转义）；null 返回 null。</summary>
        public static string? Snapshot<T>(T? entity) =>
            entity == null ? null : JsonSerializer.Serialize(entity, LogJsonOptions);

        /// <summary>构造一条操作日志并加入 DbContext（由调用方统一 SaveChanges）。登录等无 JWT 场景可用 username/name 手动补充操作人。</summary>
        public static void Add(ControllerBase controller, AppDbContext db,
            string module, string action, string detail, long durationMs,
            string? beforeJson = null, string? afterJson = null, int status = 1,
            string? username = null, string? name = null)
        {
            var user = controller.User;
            db.OperationLogs.Add(new SysOperationLog
            {
                AccountId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = username ?? user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue(JwtRegisteredClaimNames.UniqueName),
                Name = name ?? user.FindFirstValue("name") ?? user.Identity?.Name,
                Module = module,
                Action = action,
                Method = controller.Request.Method,
                Path = controller.Request.Path.Value ?? "",
                Detail = detail,
                Status = status,
                Ip = controller.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = controller.Request.Headers.UserAgent.ToString(),
                DurationMs = durationMs,
                BeforeData = beforeJson,
                AfterData = afterJson,
                CreatedAt = DateTime.Now
            });
        }
    }
}
