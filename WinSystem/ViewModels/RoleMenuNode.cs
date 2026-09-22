using System.Collections.ObjectModel;

namespace WinSystem.ViewModels
{
    /// <summary>权限树节点：对应 sys_menu 的某个菜单，可勾选，父子级联动。</summary>
    public class RoleMenuNode : ViewModelBase
    {
        private bool _isChecked;
        private bool _updating;

        public string Name { get; }
        public string? Permission { get; }
        public int Type { get; }
        public ObservableCollection<RoleMenuNode> Children { get; } = new();

        /// <summary>父节点（由构树时挂接）。</summary>
        public RoleMenuNode? Parent { get; set; }

        public RoleMenuNode(string name, string? permission, int type)
        {
            Name = name;
            Permission = permission;
            Type = type;
        }

        /// <summary>是否勾选（用户点击时联动子级，子级变化时重算父级）。</summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_updating || _isChecked == value) return;
                SetChecked(value);
            }
        }

        /// <summary>用户/程序设置勾选，并向下联动子级、向上重算父级。</summary>
        private void SetChecked(bool value)
        {
            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));

            foreach (var c in Children) c.SetChecked(value);

            // 子级变化后重算祖先
            var parent = Parent;
            while (parent != null)
            {
                parent._updating = true;
                parent._isChecked = parent.Children.Count > 0 && parent.Children.All(c => c.IsChecked);
                parent.OnPropertyChanged(nameof(IsChecked));
                parent._updating = false;
                parent = parent.Parent;
            }
        }

        /// <summary>程序初始化勾选（不触发父级联动，供加载已分配权限用）。</summary>
        public void MarkChecked(bool value)
        {
            _updating = true;
            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));
            _updating = false;
        }

        /// <summary>递归收集已勾选且带权限标识的节点。</summary>
        public void CollectChecked(List<string> result)
        {
            if (_isChecked && !string.IsNullOrEmpty(Permission)) result.Add(Permission);
            foreach (var c in Children) c.CollectChecked(result);
        }
    }
}