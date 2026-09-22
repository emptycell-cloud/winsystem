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
    [Route("api/dict-types")]
    [Authorize]
    public class DictTypesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DictTypesController(AppDbContext db) => _db = db;

        // ================= 字典类型 =================

        /// <summary>类型列表（含每种类型的数据数量）。</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysDictType>>> GetAll()
        {
            var types = await _db.DictTypes.OrderBy(t => t.SortOrder).ThenBy(t => t.Id).ToListAsync();
            var counts = await _db.DictDatas.GroupBy(d => d.DictCode)
                .Select(g => new { Code = g.Key, Count = g.Count() }).ToListAsync();
            foreach (var t in types)
            {
                t.DataCount = counts.FirstOrDefault(c => c.Code == t.Code)?.Count ?? 0;
            }
            return Ok(types);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<SysDictType>> Create(DictTypeInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("字典名称不能为空。");
            if (string.IsNullOrWhiteSpace(input.Code)) return BadRequest("字典编码不能为空。");
            if (await _db.DictTypes.AnyAsync(t => t.Code == input.Code.Trim()))
                return Conflict("该字典编码已存在。");

            var type = new SysDictType
            {
                Name = input.Name.Trim(),
                Code = input.Code.Trim(),
                Status = input.Status ?? 1,
                SortOrder = input.SortOrder ?? 0,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.DictTypes.Add(type);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "新增", $"新增字典类型：{type.Name}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(type));
            await _db.SaveChangesAsync();
            return Ok(type);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, DictTypeInput input)
        {
            var type = await _db.DictTypes.FindAsync(id);
            if (type == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(type);
            var sw = Stopwatch.StartNew();
            if (input.Name != null)
            {
                if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("字典名称不能为空。");
                type.Name = input.Name.Trim();
            }
            if (input.Code != null)
            {
                if (string.IsNullOrWhiteSpace(input.Code)) return BadRequest("字典编码不能为空。");
                if (await _db.DictTypes.AnyAsync(t => t.Code == input.Code.Trim() && t.Id != id))
                    return Conflict("该字典编码已存在。");
                if (type.Code != input.Code.Trim())
                {
                    var oldCode = type.Code;
                    type.Code = input.Code.Trim();
                    // 同步更新该类型下所有数据的外键编码
                    var datas = await _db.DictDatas.Where(d => d.DictCode == oldCode).ToListAsync();
                    foreach (var d in datas) d.DictCode = type.Code;
                }
            }
            if (input.Status.HasValue) type.Status = input.Status.Value;
            if (input.SortOrder.HasValue) type.SortOrder = input.SortOrder.Value;
            if (input.Remark != null) type.Remark = input.Remark;
            type.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "编辑", $"编辑字典类型：{type.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(type));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var type = await _db.DictTypes.FindAsync(id);
            if (type == null) return NotFound();
            var beforeJson = OperationLogHelper.Snapshot(type);
            var sw = Stopwatch.StartNew();
            _db.DictTypes.Remove(type);
            _db.DictDatas.RemoveRange(_db.DictDatas.Where(d => d.DictCode == type.Code));
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "删除", $"删除字典类型：{type.Name}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ================= 字典数据 =================

        /// <summary>按字典编码取数据列表（按 sort_order 排序）。</summary>
        [HttpGet("{code}/data")]
        public async Task<ActionResult<IEnumerable<SysDictData>>> GetData(string code)
        {
            var datas = await _db.DictDatas
                .Where(d => d.DictCode == code)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Id)
                .ToListAsync();
            return Ok(datas);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("data")]
        public async Task<ActionResult<SysDictData>> CreateData(DictDataInput input)
        {
            if (string.IsNullOrWhiteSpace(input.DictCode)) return BadRequest("字典编码不能为空。");
            if (string.IsNullOrWhiteSpace(input.Label)) return BadRequest("数据标签不能为空。");
            if (string.IsNullOrWhiteSpace(input.Value)) return BadRequest("数据键值不能为空。");
            if (await _db.DictTypes.AnyAsync(t => t.Code == input.DictCode.Trim()) == false)
                return BadRequest("字典编码不存在。");
            if (await _db.DictDatas.AnyAsync(d => d.DictCode == input.DictCode.Trim() && d.Value == input.Value.Trim()))
                return Conflict("该字典下已存在相同键值的数据。");

            var data = new SysDictData
            {
                DictCode = input.DictCode.Trim(),
                Label = input.Label.Trim(),
                Value = input.Value.Trim(),
                Color = input.Color,
                Status = input.Status ?? 1,
                SortOrder = input.SortOrder ?? 0,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.DictDatas.Add(data);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "新增", $"新增字典数据：{data.Label}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(data));
            await _db.SaveChangesAsync();
            return Ok(data);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("data/{id:long}")]
        public async Task<ActionResult> UpdateData(long id, DictDataInput input)
        {
            var data = await _db.DictDatas.FindAsync(id);
            if (data == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(data);
            var sw = Stopwatch.StartNew();
            if (input.DictCode != null)
            {
                if (await _db.DictTypes.AnyAsync(t => t.Code == input.DictCode.Trim()) == false)
                    return BadRequest("字典编码不存在。");
                data.DictCode = input.DictCode.Trim();
            }
            if (input.Label != null)
            {
                if (string.IsNullOrWhiteSpace(input.Label)) return BadRequest("数据标签不能为空。");
                data.Label = input.Label.Trim();
            }
            if (input.Value != null)
            {
                if (string.IsNullOrWhiteSpace(input.Value)) return BadRequest("数据键值不能为空。");
                if (await _db.DictDatas.AnyAsync(d => d.DictCode == data.DictCode && d.Value == input.Value.Trim() && d.Id != id))
                    return Conflict("该字典下已存在相同键值的数据。");
                data.Value = input.Value.Trim();
            }
            if (input.Color != null) data.Color = input.Color;
            if (input.Status.HasValue) data.Status = input.Status.Value;
            if (input.SortOrder.HasValue) data.SortOrder = input.SortOrder.Value;
            if (input.Remark != null) data.Remark = input.Remark;
            data.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "编辑", $"编辑字典数据：{data.Label}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(data));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("data/{id:long}")]
        public async Task<ActionResult> DeleteData(long id)
        {
            var data = await _db.DictDatas.FindAsync(id);
            if (data == null) return NotFound();
            var beforeJson = OperationLogHelper.Snapshot(data);
            var sw = Stopwatch.StartNew();
            _db.DictDatas.Remove(data);
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "数据字典", "删除", $"删除字典数据：{data.Label}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public record DictTypeInput(string? Name, string? Code, int? Status, int? SortOrder, string? Remark);
        public record DictDataInput(string? DictCode, string? Label, string? Value, string? Color, int? Status, int? SortOrder, string? Remark);
    }
}
