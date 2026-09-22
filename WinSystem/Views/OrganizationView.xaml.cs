using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WinSystem.Models;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    public partial class OrganizationView : UserControl
    {
        public OrganizationView()
        {
            InitializeComponent();
            Loaded += OrganizationView_Loaded;
        }

        /// <summary>树加载完成后展开根节点（等待布局完成以规避虚拟化延迟）。</summary>
        private void OrganizationView_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                if (OrgTree.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem root)
                {
                    root.IsExpanded = true;
                }
            }));
        }

        /// <summary>树节点选中时通知 ViewModel 更新右侧详情。</summary>
        private void OrgTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is OrganizationViewModel vm)
            {
                vm.SelectedOrganization = e.NewValue as Organization;
            }
        }
    }
}