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
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RolesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            var roles = await _db.Roles.OrderBy(r => r.SortOrder).ThenBy(r => r.Id).ToListAsync();
            return Ok(roles);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult> Create(RoleInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("角色名称不能为空。");
            if (string.IsNullOrWhiteSpace(input.Code)) return BadRequest("角色编码不能为空。");
            if (await _db.Roles.AnyAsync(r => r.Code == input.Code.Trim()))
                return Conflict("该角色编码已存在。");

            var role = new Role
            {
                Id = await NextRoleIdAsync(),
                Name = input.Name.Trim(),
                Code = input.Code.Trim(),
                Description = input.Description,
                Color = input.Color,
                DataScope = input.DataScope ?? 2,
                IsBuiltIn = 0,
                Status = input.Status ?? 1,
                SortOrder = input.SortOrder ?? 0,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Roles.Add(role);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "角色权限", "新增", $"新增角色：{role.Name}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(role));
            await _db.SaveChangesAsync();
            return Ok(role);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, RoleInput input)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(role);
            var sw = Stopwatch.StartNew();
            if (input.Name != null)
            {
                if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("角色名称不能为空。");
                role.Name = input.Name.Trim();
            }
            if (input.Code != null)
            {
                if (string.IsNullOrWhiteSpace(input.Code)) return BadRequest("角色编码不能为空。");
                if (await _db.Roles.AnyAsync(r => r.Code == input.Code.Trim() && r.Id != id))
                    return Conflict("该角色编码已存在。");
                role.Code = input.Code.Trim();
            }
            if (input.Description != null) role.Description = input.Description;
            if (input.Color != null) role.Color = input.Color;
            if (input.DataScope.HasValue) role.DataScope = input.DataScope.Value;
            if (input.Status.HasValue) role.Status = input.Status.Value;
            if (input.SortOrder.HasValue) role.SortOrder = input.SortOrder.Value;
            if (input.Remark != null) role.Remark = input.Remark;
            role.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "角色权限", "编辑", $"编辑角色：{role.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(role));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();
            if (role.IsBuiltIn == 1) return BadRequest("内置角色不可删除。");
            var beforeJson = OperationLogHelper.Snapshot(role);
            var sw = Stopwatch.StartNew();
            _db.Roles.Remove(role);
            _db.RoleMenus.RemoveRange(_db.RoleMenus.Where(m => m.RoleId == id));
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "角色权限", "删除", $"删除角色：{role.Name}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>获取角色已分配的权限标识集合（来自 sys_role_menu）。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetPermissions(string id)
        {
            var keys = await _db.RoleMenus.Where(m => m.RoleId == id).Select(m => m.MenuKey).ToListAsync();
            return Ok(keys);
        }

        /// <summary>保存角色权限：全量替换 sys_role_menu。</summary>
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}/permissions")]
        public async Task<ActionResult> SavePermissions(string id, PermissionInput input)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            var beforeKeys = await _db.RoleMenus.Where(m => m.RoleId == id).Select(m => m.MenuKey).ToListAsync();
            var afterKeys = (input.Permissions ?? new List<string>()).Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim()).Distinct().ToList();
            var sw = Stopwatch.StartNew();
            _db.RoleMenus.RemoveRange(_db.RoleMenus.Where(m => m.RoleId == id));
            var now = DateTime.Now;
            foreach (var key in afterKeys)
            {
                _db.RoleMenus.Add(new RoleMenu
                {
                    RoleId = id,
                    MenuKey = key,
                    PermissionState = input.PermissionState ?? 2,
                    CreatedAt = now
                });
            }
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "角色权限", "分配权限", $"分配角色权限：{role.Name}", sw.ElapsedMilliseconds,
                OperationLogHelper.Snapshot(beforeKeys), OperationLogHelper.Snapshot(afterKeys));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public record RoleInput(string? Name, string? Code, string? Description, string? Color,
            int? DataScope, int? Status, int? SortOrder, string? Remark);
        public record PermissionInput(List<string>? Permissions, int? PermissionState);

        private async Task<string> NextRoleIdAsync()
        {
            var ids = await _db.Roles.Select(r => r.Id).ToListAsync();
            var maxNum = ids
                .Where(x => x.StartsWith("R") && int.TryParse(x[1..], out _))
                .Select(x => int.Parse(x[1..]))
                .DefaultIfEmpty(0)
                .Max();
            return "R" + (maxNum + 1).ToString("D3");
        }
    }
}