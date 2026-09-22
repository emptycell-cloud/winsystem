using System.ComponentModel.DataAnnotations.Schema;

namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_menu 菜单表（type=1 目录，type=2 菜单页面，type=3 权限按钮；侧边栏仅加载 1/2）。</summary>
    public class Menu
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? ParentId { get; set; }
        public int Type { get; set; } = 1;
        public string? Path { get; set; }
        public string? Component { get; set; }
        public string? Icon { get; set; }
        public string? Permission { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; } = 1;
        public int IsVisible { get; set; } = 1;
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>子菜单（仅 API 返回树时填充，不映射数据库）。</summary>
        [NotMapped]
        public List<Menu> Children { get; set; } = new();

        /// <summary>上级菜单名称（仅配置列表查询时填充，不映射数据库）。</summary>
        [NotMapped]
        public string? ParentName { get; set; }
    }
}
