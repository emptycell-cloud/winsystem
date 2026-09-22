using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinSystem.Api.Data;
using WinSystem.Api.Models;
using WinSystem.Api.Services;

namespace WinSystem.Api.Controllers
{
    [ApiController]
    [Route("api/configs")]
    [Authorize]
    public class SysConfigsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SysConfigsController(AppDbContext db) => _db = db;

        /// <summary>系统参数列表（按 id 排序）。</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysConfig>>> GetAll()
        {
            var list = await _db.Configs.OrderBy(c => c.Id).ToListAsync();
            return Ok(list);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<SysConfig>> Create(ConfigInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ConfigName)) return BadRequest("参数名称不能为空。");
            if (string.IsNullOrWhiteSpace(input.ConfigKey)) return BadRequest("参数键不能为空。");
            if (await _db.Configs.AnyAsync(c => c.ConfigKey == input.ConfigKey.Trim()))
                return Conflict("该参数键已存在。");

            var cfg = new SysConfig
            {
                ConfigName = input.ConfigName.Trim(),
                ConfigKey = input.ConfigKey.Trim(),
                ConfigValue = input.ConfigValue ?? "",
                ConfigType = input.ConfigType ?? 1,
                IsSystem = input.IsSystem ?? 0,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Configs.Add(cfg);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "系统参数", "新增", $"新增参数:{cfg.ConfigName}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(cfg));
            await _db.SaveChangesAsync();
            return Ok(cfg);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, ConfigInput input)
        {
            var cfg = await _db.Configs.FindAsync(id);
            if (cfg == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(cfg);
            var sw = Stopwatch.StartNew();
            if (input.ConfigName != null)
            {
                if (string.IsNullOrWhiteSpace(input.ConfigName)) return BadRequest("参数名称不能为空。");
                cfg.ConfigName = input.ConfigName.Trim();
            }
            if (input.ConfigKey != null)
            {
                if (string.IsNullOrWhiteSpace(input.ConfigKey)) return BadRequest("参数键不能为空。");
                if (await _db.Configs.AnyAsync(c => c.ConfigKey == input.ConfigKey.Trim() && c.Id != id))
                    return Conflict("该参数键已存在。");
                cfg.ConfigKey = input.ConfigKey.Trim();
            }
            if (input.ConfigValue != null) cfg.ConfigValue = input.ConfigValue;
            if (input.ConfigType.HasValue) cfg.ConfigType = input.ConfigType.Value;
            if (input.Remark != null) cfg.Remark = input.Remark;
            cfg.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "系统参数", "编辑", $"编辑参数:{cfg.ConfigName}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(cfg));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var cfg = await _db.Configs.FindAsync(id);
            if (cfg == null) return NotFound();
            if (cfg.IsSystem == 1) return BadRequest("系统内置参数不允许删除。");
            var beforeJson = OperationLogHelper.Snapshot(cfg);
            var sw = Stopwatch.StartNew();
            _db.Configs.Remove(cfg);
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "系统参数", "删除", $"删除参数:{cfg.ConfigName}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public record ConfigInput(string? ConfigName, string? ConfigKey, string? ConfigValue, int? ConfigType, int? IsSystem, string? Remark);
    }
}
