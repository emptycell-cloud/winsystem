using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WinSystem.Views;

namespace WinSystem.Views
{
    /// <summary>消息框类型。</summary>
    public enum MessageDialogType
    {
        Info,
        Success,
        Warning,
        Error,
        Question
    }

    /// <summary>自定义美化的消息对话框。</summary>
    public partial class MessageDialogWindow : Window
    {
        public MessageDialogWindow(string message, string title, MessageDialogType type, bool showCancel)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
            Title = title;
            ApplyType(type);
            if (showCancel) CancelBtn.Visibility = Visibility.Visible;
            else OkBtn.Content = "知道了";
        }

        private void ApplyType(MessageDialogType type)
        {
            Brush fill;
            string icon;
            switch (type)
            {
                case MessageDialogType.Success:
                    fill = (Brush)Application.Current.FindResource("SuccessBrush");
                    icon = "M5,12.5 L10.2,17.5 L19,7.5"; // 对勾
                    break;
                case MessageDialogType.Error:
                    fill = (Brush)Application.Current.FindResource("DangerBrush");
                    icon = "M12,6 V14 M12,17.5 V17.6"; // 惊叹号
                    break;
                case MessageDialogType.Warning:
                    fill = (Brush)Application.Current.FindResource("WarningBrush");
                    icon = "M12,6 V14 M12,17.5 V17.6"; // 惊叹号
                    break;
                case MessageDialogType.Question:
                    fill = (Brush)Application.Current.FindResource("PrimaryBrush");
                    icon = "M9.5,9.5 A2.5,2.5 0 1 1 14.5,10 C14.5,11.5 12,11.5 12,13.5 V14.5"; // 问号
                    break;
                default:
                    fill = (Brush)Application.Current.FindResource("InfoBrush");
                    icon = "M12,7.5 V12 M12,16.5 V16.6"; // i
                    break;
            }

            IconHost.Background = fill;
            IconPath.Data = Geometry.Parse(icon);
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
            DialogResult = true; Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}