using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;
using WinSystem.Api.Models;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/operation-logs")]
    [Authorize]
    public class OperationLogsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OperationLogsController(AppDbContext db) => _db = db;

        /// <summary>操作日志列表（审计数据，仅管理员），按时间倒序。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysOperationLog>>> GetAll()
        {
            var list = await _db.OperationLogs
                .OrderByDescending(l => l.Id)
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>删除单条日志。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var log = await _db.OperationLogs.FindAsync(id);
            if (log == null) return NotFound();
            _db.OperationLogs.Remove(log);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>清空全部操作日志。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        public async Task<ActionResult> ClearAll()
        {
            var logs = await _db.OperationLogs.ToListAsync();
            _db.OperationLogs.RemoveRange(logs);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
