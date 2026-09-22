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
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OrganizationsController(AppDbContext db) => _db = db;

        /// <summary>返回组织树形结构（仅启用，按 sort_order 排序），供用户编辑等树形选择使用。</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organization>>> GetTree()
        {
            var orgs = await _db.Organizations
                .Where(o => o.Status == 1)
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Id)
                .ToListAsync();

            var nodes = orgs.ToDictionary(o => o.Id);
            var roots = new List<Organization>();
            foreach (var org in orgs)
            {
                if (!string.IsNullOrEmpty(org.ParentId) && nodes.TryGetValue(org.ParentId, out var parent))
                {
                    parent.Children.Add(org);
                }
                else
                {
                    roots.Add(org);
                }
            }
            return Ok(roots);
        }

        /// <summary>返回全部组织扁平列表（含禁用，带上级名称），供组织机构配置页使用。</summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Organization>>> GetAll()
        {
            var orgs = await _db.Organizations
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Id)
                .ToListAsync();

            var names = orgs.ToDictionary(o => o.Id, o => o.Name);
            foreach (var org in orgs)
            {
                if (!string.IsNullOrEmpty(org.ParentId) && names.TryGetValue(org.ParentId, out var parentName))
                    org.ParentName = parentName;
            }
            return Ok(orgs);
        }

        public record OrgInput(string? Name, string? Code, string? ParentId, int? Type, string? Manager,
            string? Phone, string? Email, int? SortOrder, int? Status, string? Description);

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<Organization>> Create(OrgInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("组织名称不能为空。");

            var parentId = string.IsNullOrWhiteSpace(input.ParentId) ? null : input.ParentId.Trim();
            if (parentId != null && await _db.Organizations.FindAsync(parentId) == null)
                return BadRequest("上级组织不存在。");

            var org = new Organization
            {
                Id = await NextOrgIdAsync(parentId),
                Code = input.Code?.Trim() ?? "",
                Name = input.Name.Trim(),
                ParentId = parentId,
                Type = input.Type ?? 1,
                Manager = input.Manager,
                Phone = input.Phone,
                Email = input.Email,
                SortOrder = input.SortOrder ?? 0,
                Status = input.Status ?? 1,
                Description = input.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Organizations.Add(org);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "组织机构", "新增", $"新增组织：{org.Name}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(org));
            await _db.SaveChangesAsync();
            return Ok(org);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, OrgInput input)
        {
            var org = await _db.Organizations.FindAsync(id);
            if (org == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(org);
            var sw = Stopwatch.StartNew();
            if (input.Name != null)
            {
                if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("组织名称不能为空。");
                org.Name = input.Name.Trim();
            }
            if (input.Code != null) org.Code = input.Code.Trim();
            if (input.ParentId != null)
            {
                var parentId = string.IsNullOrWhiteSpace(input.ParentId) ? null : input.ParentId.Trim();
                if (parentId == org.Id) return BadRequest("上级组织不能是自己。");
                if (parentId != null && await _db.Organizations.FindAsync(parentId) == null)
                    return BadRequest("上级组织不存在。");
                org.ParentId = parentId;
            }
            if (input.Type.HasValue) org.Type = input.Type.Value;
            if (input.Manager != null) org.Manager = input.Manager;
            if (input.Phone != null) org.Phone = input.Phone;
            if (input.Email != null) org.Email = input.Email;
            if (input.SortOrder.HasValue) org.SortOrder = input.SortOrder.Value;
            if (input.Status.HasValue) org.Status = input.Status.Value;
            if (input.Description != null) org.Description = input.Description;
            org.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "组织机构", "编辑", $"编辑组织：{org.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(org));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var org = await _db.Organizations.FindAsync(id);
            if (org == null) return NotFound();
            if (await _db.Organizations.AnyAsync(o => o.ParentId == id))
                return BadRequest("该组织下存在下级组织，请先删除下级组织。");
            var beforeJson = OperationLogHelper.Snapshot(org);
            var sw = Stopwatch.StartNew();
            _db.Organizations.Remove(org);
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "组织机构", "删除", $"删除组织：{org.Name}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>生成组织 ID：根级取最大数值 +1；子级为父 ID + 两位序号（10→1001、1002…）。</summary>
        private async Task<string> NextOrgIdAsync(string? parentId)
        {
            var ids = await _db.Organizations.Select(o => o.Id).ToListAsync();
            if (string.IsNullOrEmpty(parentId))
            {
                var max = ids.Where(x => int.TryParse(x, out _)).Select(int.Parse).DefaultIfEmpty(0).Max();
                return (max + 1).ToString();
            }

            var prefix = parentId;
            var maxNum = ids
                .Where(x => x.StartsWith(prefix, StringComparison.Ordinal) &&
                            x.Length == prefix.Length + 2 &&
                            int.TryParse(x[prefix.Length..], out _))
                .Select(x => int.Parse(x[prefix.Length..]))
                .DefaultIfEmpty(0)
                .Max();
            return prefix + (maxNum + 1).ToString("D2");
        }
    }
}
