using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinSystem.Models
{
    /// <summary>侧边菜单（由 API 从 sys_menu 加载，type=1 目录 / type=2 菜单页面 / type=3 权限按钮）。</summary>
    public class Menu : INotifyPropertyChanged
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

        private string? _parentName;
        /// <summary>上级菜单名称（方向面板即时展示）。</summary>
        public string? ParentName
        {
            get => _parentName;
            set { if (_parentName == value) return; _parentName = value; OnPropertyChanged(); }
        }

        /// <summary>子菜单（API 返回的树形子节点）。</summary>
        public List<Menu> Children { get; set; } = new();

        /// <summary>类型文字：1目录 2菜单页面 3权限按钮。</summary>
        public string TypeText => Type switch
        {
            1 => "目录",
            2 => "菜单页面",
            3 => "权限按钮",
            _ => "未知"
        };

        /// <summary>状态文字：1启用 0禁用。</summary>
        public string StatusText => Status == 1 ? "启用" : "禁用";

        /// <summary>显示文字：1显示 0隐藏。</summary>
        public string VisibleText => IsVisible == 1 ? "显示" : "隐藏";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
