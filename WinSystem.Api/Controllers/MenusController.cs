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
    public class MenusController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MenusController(AppDbContext db) => _db = db;

        /// <summary>返回侧边菜单树（仅加载 type=1 目录 与 type=2 菜单页面，按 sort_order 排序）。</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetTree()
        {
            var menus = await _db.Menus
                .Where(m => m.Type == 1 || m.Type == 2)
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Id)
                .ToListAsync();

            // 构建树：根节点 parent_id 为 NULL，type=2 挂到对应父级下
            var roots = new List<Menu>();
            foreach (var menu in menus)
            {
                if (!string.IsNullOrEmpty(menu.ParentId))
                {
                    var parent = menus.FirstOrDefault(x => x.Id == menu.ParentId);
                    if (parent != null) parent.Children.Add(menu);
                    else roots.Add(menu);
                }
                else
                {
                    roots.Add(menu);
                }
            }
            return Ok(roots);
        }

        /// <summary>返回全部菜单扁平列表（含 type=3 权限按钮，带上级菜单名称），供菜单配置页使用。</summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Menu>>> GetAll()
        {
            var menus = await _db.Menus
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Id)
                .ToListAsync();

            var names = menus.ToDictionary(m => m.Id, m => m.Name);
            foreach (var menu in menus)
            {
                if (!string.IsNullOrEmpty(menu.ParentId) && names.TryGetValue(menu.ParentId, out var parentName))
                    menu.ParentName = parentName;
            }
            return Ok(menus);
        }

        public record MenuInput(string? Name, string? ParentId, int? Type, string? Path, string? Component,
            string? Icon, string? Permission, int? SortOrder, int? Status, int? IsVisible, string? Remark);

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult> Create(MenuInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("菜单名称不能为空。");
            var type = input.Type ?? 2;
            if (type is < 1 or > 3) return BadRequest("菜单类型不合法。");

            var parentId = string.IsNullOrWhiteSpace(input.ParentId) ? null : input.ParentId.Trim();
            if (parentId != null && await _db.Menus.FindAsync(parentId) == null)
                return BadRequest("上级菜单不存在。");

            var menu = new Menu
            {
                Id = await NextMenuIdAsync(parentId),
                Name = input.Name.Trim(),
                ParentId = parentId,
                Type = type,
                Path = input.Path,
                Component = input.Component,
                Icon = input.Icon,
                Permission = input.Permission,
                SortOrder = input.SortOrder ?? 0,
                Status = input.Status ?? 1,
                IsVisible = input.IsVisible ?? 1,
                Remark = input.Remark,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Menus.Add(menu);
            var sw = Stopwatch.StartNew();
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "菜单配置", "新增", $"新增菜单：{menu.Name}", sw.ElapsedMilliseconds,
                afterJson: OperationLogHelper.Snapshot(menu));
            await _db.SaveChangesAsync();
            return Ok(menu);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, MenuInput input)
        {
            var menu = await _db.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            var beforeJson = OperationLogHelper.Snapshot(menu);
            var sw = Stopwatch.StartNew();
            if (input.Name != null)
            {
                if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest("菜单名称不能为空。");
                menu.Name = input.Name.Trim();
            }
            if (input.Type.HasValue)
            {
                if (input.Type.Value is < 1 or > 3) return BadRequest("菜单类型不合法。");
                menu.Type = input.Type.Value;
            }
            if (input.ParentId != null)
            {
                var parentId = string.IsNullOrWhiteSpace(input.ParentId) ? null : input.ParentId.Trim();
                if (parentId == menu.Id) return BadRequest("上级菜单不能是自己。");
                if (parentId != null && await _db.Menus.FindAsync(parentId) == null)
                    return BadRequest("上级菜单不存在。");
                menu.ParentId = parentId;
            }
            if (input.Path != null) menu.Path = input.Path;
            if (input.Component != null) menu.Component = input.Component;
            if (input.Icon != null) menu.Icon = input.Icon;
            if (input.Permission != null) menu.Permission = input.Permission;
            if (input.SortOrder.HasValue) menu.SortOrder = input.SortOrder.Value;
            if (input.Status.HasValue) menu.Status = input.Status.Value;
            if (input.IsVisible.HasValue) menu.IsVisible = input.IsVisible.Value;
            if (input.Remark != null) menu.Remark = input.Remark;
            menu.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "菜单配置", "编辑", $"编辑菜单：{menu.Name}", sw.ElapsedMilliseconds,
                beforeJson, OperationLogHelper.Snapshot(menu));
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var menu = await _db.Menus.FindAsync(id);
            if (menu == null) return NotFound();
            if (await _db.Menus.AnyAsync(m => m.ParentId == id))
                return BadRequest("该菜单下存在子菜单，请先删除子菜单。");
            var beforeJson = OperationLogHelper.Snapshot(menu);
            var sw = Stopwatch.StartNew();
            _db.Menus.Remove(menu);
            await _db.SaveChangesAsync();
            sw.Stop();
            OperationLogHelper.Add(this, _db, "菜单配置", "删除", $"删除菜单：{menu.Name}", sw.ElapsedMilliseconds,
                beforeJson);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>生成菜单 ID：根级 M01、M02…，子级为父 ID + 两位序号（M0201、M020101…）。</summary>
        private async Task<string> NextMenuIdAsync(string? parentId)
        {
            var ids = await _db.Menus.Select(m => m.Id).ToListAsync();
            var prefix = string.IsNullOrEmpty(parentId) ? "M" : parentId;
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
