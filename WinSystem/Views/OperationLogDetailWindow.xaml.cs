using System.Windows;
using System.Windows.Input;

namespace WinSystem.Views
{
    /// <summary>操作日志详情窗口（只读展示单条操作日志）。</summary>
    public partial class OperationLogDetailWindow : Window
    {
        public OperationLogDetailWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
