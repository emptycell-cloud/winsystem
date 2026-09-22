using System.Collections.ObjectModel;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>菜单配置编辑表单：新增/编辑菜单记录（sys_menu）。</summary>
    public class MenuEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] TypeOptions = { "目录", "菜单页面", "权限按钮" };
        public static readonly string[] VisibleOptions = { "显示", "隐藏" };

        private readonly bool _isNew;
        private readonly List<Menu> _all;
        private string _name;
        private string _path;
        private string _component;
        private string _icon;
        private string _permission;
        private string _remark;
        private string _sortOrderText;
        private string? _parentId;
        private int _type;
        private int _status;
        private int _visible;
        private Menu? _selectedParentMenu;

        public MenuEditorViewModel(Menu? menu, List<Menu> all)
        {
            _isNew = menu == null;
            _all = all;
            _name = menu?.Name ?? "";
            _path = menu?.Path ?? "";
            _component = menu?.Component ?? "";
            _icon = menu?.Icon ?? "";
            _permission = menu?.Permission ?? "";
            _remark = menu?.Remark ?? "";
            _sortOrderText = (menu?.SortOrder ?? 0).ToString();
            _parentId = menu?.ParentId;
            _type = menu?.Type ?? 2;
            _status = menu?.Status ?? 1;
            _visible = menu?.IsVisible ?? 1;
            Title = _isNew ? "新增菜单" : "编辑菜单";

            BuildParentTree(menu?.Id);
            // 反向选中当前上级菜单（根级无上级则不选中树节点）
            var found = FindMenu(_parentId);
            if (found != null) SelectedParentMenu = found;
        }

        public static MenuEditorViewModel CreateNew(List<Menu> all) => new(null, all);

        /// <summary>上级菜单树（编辑时排除自身及子孙），供树形控件绑定。</summary>
        public ObservableCollection<Menu> ParentTree { get; } = new();

        /// <summary>上级菜单选中项；树节点选中时更新 _parentId。</summary>
        public Menu? SelectedParentMenu
        {
            get => _selectedParentMenu;
            set { if (SetProperty(ref _selectedParentMenu, value)) _parentId = value?.Id; }
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Path { get => _path; set => SetProperty(ref _path, value); }
        public string Component { get => _component; set => SetProperty(ref _component, value); }
        public string Icon { get => _icon; set => SetProperty(ref _icon, value); }
        public string Permission { get => _permission; set => SetProperty(ref _permission, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        /// <summary>排序号（文本框字符串，便于输入）。</summary>
        public string SortOrderText
        {
            get => _sortOrderText;
            set => SetProperty(ref _sortOrderText, value);
        }

        public string Type
        {
            get => TypeOptions[Math.Clamp(_type - 1, 0, TypeOptions.Length - 1)];
            set { var i = Array.IndexOf(TypeOptions, value); if (i >= 0) _type = i + 1; }
        }

        /// <summary>是否显示：1=显示 0=隐藏。</summary>
        public string Visible
        {
            get => VisibleOptions[_visible == 1 ? 0 : 1];
            set { var i = Array.IndexOf(VisibleOptions, value); if (i >= 0) _visible = i == 0 ? 1 : 0; }
        }

        public bool IsNew => _isNew;

        /// <summary>构建上级菜单树：按 ParentId 组装层级，编辑时排除自身及其子孙。</summary>
        private void BuildParentTree(string? excludeId)
        {
            foreach (var m in _all) m.Children.Clear();

            var excluded = new HashSet<string>();
            if (!string.IsNullOrEmpty(excludeId))
            {
                excluded.Add(excludeId);
                void CollectDescendants(string pid)
                {
                    foreach (var m in _all.Where(x => x.ParentId == pid))
                    {
                        if (excluded.Add(m.Id)) CollectDescendants(m.Id);
                    }
                }
                CollectDescendants(excludeId);
            }

            var nodes = _all.Where(m => !excluded.Contains(m.Id)).ToList();
            var byId = nodes.ToDictionary(m => m.Id);
            ParentTree.Clear();
            foreach (var m in nodes)
            {
                if (!string.IsNullOrEmpty(m.ParentId) && byId.TryGetValue(m.ParentId, out var parent))
                    parent.Children.Add(m);
                else
                    ParentTree.Add(m);
            }
        }

        /// <summary>在上级菜单树中按 Id 查找节点（返回 null 表示根级）。</summary>
        private Menu? FindMenu(string? id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var root in ParentTree)
            {
                var found = FindRecursive(root, id);
                if (found != null) return found;
            }
            return null;
        }

        private static Menu? FindRecursive(Menu node, string id)
        {
            if (node.Id == id) return node;
            foreach (var child in node.Children)
            {
                var found = FindRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }

        private int SortValue => int.TryParse(SortOrderText?.Trim(), out var n) ? n : 0;

        public Menu BuildEntity() => new()
        {
            Name = Name.Trim(),
            ParentId = _parentId,
            Type = _type,
            Path = Path.Trim(),
            Component = Component.Trim(),
            Icon = Icon.Trim(),
            Permission = Permission.Trim(),
            SortOrder = SortValue,
            Status = _status,
            IsVisible = _visible,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(Menu menu)
        {
            menu.Name = Name.Trim();
            menu.ParentId = _parentId;
            menu.Type = _type;
            menu.Path = Path.Trim();
            menu.Component = Component.Trim();
            menu.Icon = Icon.Trim();
            menu.Permission = Permission.Trim();
            menu.SortOrder = SortValue;
            menu.Status = _status;
            menu.IsVisible = _visible;
            menu.Remark = Remark.Trim();
            menu.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写菜单名称。"; return false; }
            if (_parentId == null && _type == 2) { Message = "菜单页面必须选择上级目录。"; return false; }
            Message = "";
            return true;
        }
    }
}
