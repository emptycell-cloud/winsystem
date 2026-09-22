using System.Collections.ObjectModel;
using WinSystem.Models;
using WinSystem.Services;

namespace WinSystem.ViewModels
{
    /// <summary>角色分配权限表单：树形勾选 sys_menu 的权限标识，加载已分配项并返回选中集合。</summary>
    public class RolePermissionViewModel : ViewModelBase
    {
        private readonly string _roleName;

        public RolePermissionViewModel(Role role, List<string> assigned)
        {
            _roleName = role.Name;
            Title = $"分配权限 - {role.Name}";
            Assigned = assigned;
            BuildTree();
            MarkAssigned();
        }

        /// <summary>窗口标题。</summary>
        public string Title { get; }

        /// <summary>已分配的权限标识。</summary>
        public List<string> Assigned { get; }

        /// <summary>权限树（对应 sys_menu 层级）。</summary>
        public ObservableCollection<RoleMenuNode> ParentTree { get; } = new();

        /// <summary>已勾选的权限标识集合。</summary>
        public List<string> SelectedPermissions
        {
            get
            {
                var result = new List<string>();
                foreach (var root in ParentTree) root.CollectChecked(result);
                return result;
            }
        }

        /// <summary>依据 AllMenus（含 permission）独立按 ParentId 组装权限树，不依赖共享的 Children。</summary>
        private void BuildTree()
        {
            var menus = Session.Data.AllMenus.ToList();
            var byId = menus.ToDictionary(m => m.Id, m => new RoleMenuNode(m.Name, m.Permission, m.Type));
            foreach (var menu in menus)
            {
                var node = byId[menu.Id];
                if (!string.IsNullOrEmpty(menu.ParentId) && byId.TryGetValue(menu.ParentId, out var parent))
                {
                    parent.Children.Add(node);
                    node.Parent = parent;
                }
                else
                {
                    ParentTree.Add(node);
                }
            }
        }

        /// <summary>将已分配权限勾选到对应节点。</summary>
        private void MarkAssigned()
        {
            var set = new HashSet<string>(Assigned ?? new List<string>());
            void Mark(RoleMenuNode node)
            {
                if (!string.IsNullOrEmpty(node.Permission) && set.Contains(node.Permission))
                    node.MarkChecked(true);
                foreach (var c in node.Children) Mark(c);
            }
            foreach (var root in ParentTree) Mark(root);
        }
    }
}