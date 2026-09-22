using System.Windows;
using System.Windows.Media;
using WinSystem.Models;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    /// <summary>集中式对话框服务。</summary>
    public static class DialogService
    {
        private static void Prepare(Window win)
        {
            var owner = Application.Current?.MainWindow;
            if (owner != null && owner != win)
            {
                win.Owner = owner;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        public static bool? ShowUserEditor(UserEditorViewModel vm)
        {
            var win = new UserEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowProductEditor(ProductEditorViewModel vm)
        {
            var win = new ProductEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowOrderEditor(OrderEditorViewModel vm)
        {
            var win = new OrderEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowMenuEditor(MenuEditorViewModel vm)
        {
            var win = new MenuEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowOrganizationEditor(OrganizationEditorViewModel vm)
        {
            var win = new OrganizationEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowRoleEditor(RoleEditorViewModel vm)
        {
            var win = new RoleEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowRolePermission(RolePermissionViewModel vm)
        {
            var win = new RolePermissionWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowMessageEditor(MessageEditorViewModel vm)
        {
            var win = new MessageEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowDictTypeEditor(DictTypeEditorViewModel vm)
        {
            var win = new DictTypeEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowDictDataEditor(DictDataEditorViewModel vm)
        {
            var win = new DictDataEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowConfigEditor(ConfigEditorViewModel vm)
        {
            var win = new ConfigEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static bool? ShowScheduleEditor(ScheduleEditorViewModel vm)
        {
            var win = new ScheduleEditorWindow { DataContext = vm };
            Prepare(win);
            return win.ShowDialog();
        }

        public static void ShowMessageDetail(MessageDetailViewModel vm)
        {
            var win = new MessageDetailWindow { DataContext = vm };
            Prepare(win);
            win.ShowDialog();
        }

        public static void ShowOperationLogDetail(SysOperationLog log)
        {
            var win = new OperationLogDetailWindow { DataContext = log };
            Prepare(win);
            win.ShowDialog();
        }

        public static bool Confirm(string message, string title = "确认")
        {
            return Show(message, title, MessageDialogType.Question, true) is true;
        }

        public static void Info(string message, string title = "提示")
        {
            Show(message, title, MessageDialogType.Info, false);
        }

        public static void Success(string message, string title = "成功")
        {
            Show(message, title, MessageDialogType.Success, false);
        }

        public static void Error(string message, string title = "错误")
        {
            Show(message, title, MessageDialogType.Error, false);
        }

        private static bool? Show(string message, string title, MessageDialogType type, bool showCancel)
        {
            var win = new MessageDialogWindow(message, title, type, showCancel);
            Prepare(win);
            return win.ShowDialog();
        }

        public static Brush Color(string key) => (Brush)Application.Current.FindResource(key);
    }
}