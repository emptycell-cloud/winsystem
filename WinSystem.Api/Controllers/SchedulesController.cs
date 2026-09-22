using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;
using WinSystem.Api.Models;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private static readonly JsonSerializerOptions LogJsonOptions = new()
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // 快照数据直接保存中文，不做 \uXXXX 转义，便于日志阅读
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly AppDbContext _db;
        public SchedulesController(AppDbContext db) => _db = db;

        /// <summary>日程列表（仅正常状态，按日期、开始时间排序）。</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysSchedule>>> GetAll()
        {
            var list = await _db.Schedules
                .Where(s => s.Status == 1)
                .OrderBy(s => s.ScheduleDate)
                .ThenBy(s => s.StartTime)
                .ThenBy(s => s.Id)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<SysSchedule>> Create(ScheduleInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("日程标题不能为空。");
            if (input.ScheduleDate == null) return BadRequest("日程日期不能为空。");

            var sw = Stopwatch.StartNew();
            var schedule = new SysSchedule
            {
                Title = input.Title.Trim(),
                Description = input.Description,
                ScheduleDate = input.ScheduleDate.Value.Date,
                StartTime = string.IsNullOrWhiteSpace(input.StartTime) ? null : input.StartTime.Trim(),
                EndTime = string.IsNullOrWhiteSpace(input.EndTime) ? null : input.EndTime.Trim(),
                Color = string.IsNullOrWhiteSpace(input.Color) ? "#3B82F6" : input.Color.Trim(),
                Location = input.Location,
                IsCompleted = input.IsCompleted ?? 0,
                CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatorName = User.Identity?.Name,
                Status = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
            sw.Stop();

            await LogOperationAsync(null, schedule, "新增", $"新增日程：{schedule.Title}", sw.ElapsedMilliseconds);
            return Ok(schedule);
        }

        /// <summary>当前用户是否为管理员（可管理全部日程）。</summary>
        private bool IsAdmin => User.IsInRole("super_admin") || User.IsInRole("admin");

        /// <summary>当前用户是否为日程创建人。</summary>
        private bool IsOwner(SysSchedule s) =>
            s.CreatorId != null && s.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>归属校验：非管理员只能操作自己创建的日程，防止越权修改他人数据。</summary>
        private ActionResult? OwnershipGuard(SysSchedule s) =>
            IsAdmin || IsOwner(s) ? null : StatusCode(StatusCodes.Status403Forbidden, "无权操作他人日程。");

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, ScheduleInput input)
        {
            var schedule = await _db.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (OwnershipGuard(schedule) is { } guard) return guard;
            if (schedule.Status != 1) return BadRequest("该日程已删除。");

            var before = Snapshot(schedule);
            var sw = Stopwatch.StartNew();
            if (input.Title != null)
            {
                if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("日程标题不能为空。");
                schedule.Title = input.Title.Trim();
            }
            if (input.ScheduleDate != null) schedule.ScheduleDate = input.ScheduleDate.Value.Date;
            if (input.Description != null) schedule.Description = input.Description;
            if (input.StartTime != null) schedule.StartTime = string.IsNullOrWhiteSpace(input.StartTime) ? null : input.StartTime.Trim();
            if (input.EndTime != null) schedule.EndTime = string.IsNullOrWhiteSpace(input.EndTime) ? null : input.EndTime.Trim();
            if (input.Color != null) schedule.Color = string.IsNullOrWhiteSpace(input.Color) ? "#3B82F6" : input.Color.Trim();
            if (input.Location != null) schedule.Location = input.Location;
            if (input.IsCompleted.HasValue) schedule.IsCompleted = input.IsCompleted.Value;
            schedule.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();

            await LogOperationAsync(before, schedule, "编辑", $"编辑日程：{schedule.Title}", sw.ElapsedMilliseconds);
            return NoContent();
        }

        /// <summary>删除日程（软删除：状态置为 2）。</summary>
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var schedule = await _db.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (OwnershipGuard(schedule) is { } guard) return guard;

            var before = Snapshot(schedule);
            var sw = Stopwatch.StartNew();
            schedule.Status = 2;
            schedule.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            sw.Stop();

            await LogOperationAsync(before, null, "删除", $"删除日程：{schedule.Title}", sw.ElapsedMilliseconds);
            return NoContent();
        }

        /// <summary>浅拷贝实体快照，供操作日志记录修改前数据。</summary>
        private static SysSchedule Snapshot(SysSchedule s)
            => JsonSerializer.Deserialize<SysSchedule>(JsonSerializer.Serialize(s, LogJsonOptions), LogJsonOptions)!;

        /// <summary>写入操作日志：模块为“日程管理”，保存修改前后数据切片。</summary>
        private async Task LogOperationAsync(SysSchedule? before, SysSchedule? after, string action, string detail, long durationMs)
        {
            try
            {
                _db.OperationLogs.Add(new SysOperationLog
                {
                    AccountId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.UniqueName),
                    Name = User.FindFirstValue("name") ?? User.Identity?.Name,
                    Module = "日程管理",
                    Action = action,
                    Method = Request.Method,
                    Path = Request.Path.Value ?? "/api/schedules",
                    Detail = detail,
                    Status = 1,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    DurationMs = durationMs,
                    BeforeData = before == null ? null : JsonSerializer.Serialize(before, LogJsonOptions),
                    AfterData = after == null ? null : JsonSerializer.Serialize(after, LogJsonOptions),
                    CreatedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            catch { /* 日志写入失败不影响业务主流程 */ }
        }

        public record ScheduleInput(string? Title, string? Description, DateTime? ScheduleDate,
            string? StartTime, string? EndTime, string? Color, string? Location, int? IsCompleted);
    }
}
