using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WinSystem.Models;
using WinSystem.ViewModels;
using MenuModel = WinSystem.Models.Menu;

namespace WinSystem.Views
{
    public partial class MenuEditorWindow : Window
    {
        public MenuEditorWindow()
        {
            InitializeComponent();
            Loaded += MenuEditorWindow_Loaded;
        }

        /// <summary>窗口加载后反向展开并选中上级菜单节点（TreeView 容器需布局完成后才生成）。</summary>
        private void MenuEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MenuEditorViewModel vm || vm.SelectedParentMenu == null) return;
            Dispatcher.BeginInvoke(new Action(() => ExpandAndSelect(vm.SelectedParentMenu)));
        }

        /// <summary>展开祖先节点并选中目标菜单节点（分步等待容器生成）。</summary>
        private void ExpandAndSelect(MenuModel target)
        {
            var path = new List<MenuModel>();
            MenuModel? current = target;
            while (current != null)
            {
                path.Insert(0, current);
                current = FindParent(ParentMenuTree.ItemsSource as IEnumerable, current);
            }
            ExpandFrom(ParentMenuTree, path, 0, target);
        }

        private void ExpandFrom(ItemsControl parent, List<MenuModel> path, int level, MenuModel target)
        {
            if (level >= path.Count) return;

            var node = path[level];
            var container = FindContainer(parent, node);
            if (container == null)
            {
                Dispatcher.BeginInvoke(new Action(() => ExpandFrom(parent, path, level, target)), DispatcherPriority.Background);
                return;
            }

            if (level == path.Count - 1)
            {
                container.IsSelected = true;
                container.BringIntoView();
                return;
            }

            container.IsExpanded = true;
            Dispatcher.BeginInvoke(new Action(() => ExpandFrom(container, path, level + 1, target)), DispatcherPriority.Background);
        }

        private static TreeViewItem? FindContainer(ItemsControl root, MenuModel node)
        {
            foreach (var item in root.Items)
            {
                if (item is not MenuModel menu || menu.Id != node.Id) continue;
                return root.ItemContainerGenerator.ContainerFromItem(menu) as TreeViewItem;
            }
            return null;
        }

        private static MenuModel? FindParent(IEnumerable? items, MenuModel child)
        {
            if (items == null) return null;
            foreach (var item in items)
            {
                if (item is not MenuModel menu) continue;
                if (menu.Children.Any(c => c.Id == child.Id)) return menu;
                var found = FindParent(menu.Children, child);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>树节点选中时通知 ViewModel 更新当前上级菜单。</summary>
        private void ParentMenuTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MenuEditorViewModel vm && e.NewValue is MenuModel menu)
                vm.SelectedParentMenu = menu;
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
            if (DataContext is not MenuEditorViewModel vm) return;
            if (vm.Validate()) { DialogResult = true; Close(); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}