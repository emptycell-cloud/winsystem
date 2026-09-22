using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_organization 组织机构表（树形结构）。</summary>
    public class Organization : INotifyPropertyChanged
    {
        public string Id { get; set; } = "";
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? ParentId { get; set; }
        public int Type { get; set; } = 1;
        public string? Manager { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; } = 1;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        private string? _parentName;
        /// <summary>上级组织名称（方向面板即时展示）。</summary>
        public string? ParentName
        {
            get => _parentName;
            set { if (_parentName == value) return; _parentName = value; OnPropertyChanged(); }
        }

        /// <summary>子组织。</summary>
        public ObservableCollection<Organization> Children { get; set; } = new();

        /// <summary>类型文字：1总部 2中心 3部门。</summary>
        public string TypeText => Type switch
        {
            1 => "总部",
            2 => "中心",
            3 => "部门",
            _ => "其他"
        };

        /// <summary>状态文字：1启用 0禁用。</summary>
        public string StatusText => Status == 1 ? "启用" : "禁用";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
