using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WinSystem.Models;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    public partial class UserEditorWindow : Window
    {
        public UserEditorWindow()
        {
            InitializeComponent();
            Loaded += UserEditorWindow_Loaded;
        }

        private void UserEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not UserEditorViewModel vm || vm.SelectedOrg == null) return;
            // 延迟到布局完成后再展开/选中（TreeView 容器需要布局后才生成）
            Dispatcher.BeginInvoke(new Action(() => ExpandAndSelect(vm.SelectedOrg!)));
        }

        /// <summary>展开祖先节点并选中目标组织节点（分步等待容器生成）。</summary>
        private void ExpandAndSelect(Organization target)
        {
            var path = new List<Organization>();
            Organization? current = target;
            while (current != null)
            {
                path.Insert(0, current);
                current = FindParent(OrgTree.ItemsSource as IEnumerable, current);
            }

            ExpandFrom(OrgTree, path, 0, target);
        }

        /// <summary>从父容器逐级展开 path 中的节点，最后选中目标。</summary>
        private void ExpandFrom(ItemsControl parent, List<Organization> path, int level, Organization target)
        {
            if (level >= path.Count) return;

            var node = path[level];
            var container = FindContainer(parent, node);
            if (container == null)
            {
                // 容器尚未生成（布局未完成），稍后重试
                Dispatcher.BeginInvoke(new Action(() => ExpandFrom(parent, path, level, target)), DispatcherPriority.Background);
                return;
            }

            if (level == path.Count - 1)
            {
                // 目标节点：选中并滚动到可见
                container.IsSelected = true;
                container.BringIntoView();
                return;
            }

            // 祖先节点：展开后下钻
            container.IsExpanded = true;
            Dispatcher.BeginInvoke(new Action(() => ExpandFrom(container, path, level + 1, target)), DispatcherPriority.Background);
        }

        private static TreeViewItem? FindContainer(ItemsControl root, Organization node)
        {
            foreach (var item in root.Items)
            {
                if (item is not Organization org || org.Id != node.Id) continue;
                return root.ItemContainerGenerator.ContainerFromItem(org) as TreeViewItem;
            }
            return null;
        }

        private static Organization? FindParent(IEnumerable? items, Organization child)
        {
            if (items == null) return null;
            foreach (var item in items)
            {
                if (item is not Organization org) continue;
                if (org.Children.Any(c => c.Id == child.Id)) return org;
                var found = FindParent(org.Children, child);
                if (found != null) return found;
            }
            return null;
        }

        private void OrgTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is UserEditorViewModel vm && e.NewValue is Organization org)
                vm.SelectedOrg = org;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { DialogResult = false; Close(); }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not UserEditorViewModel vm) return;
            if (vm.Validate()) { DialogResult = true; Close(); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}