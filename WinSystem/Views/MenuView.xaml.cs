using System.Windows;
using System.Windows.Controls;
using WinSystem.Models;
using WinSystem.ViewModels;
using MenuModel = WinSystem.Models.Menu;

namespace WinSystem.Views
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        /// <summary>树节点选中时通知 ViewModel 更新右侧详情。</summary>
        private void MenuTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MenuViewModel vm)
            {
                vm.SelectedMenu = e.NewValue as MenuModel;
            }
        }
    }
}