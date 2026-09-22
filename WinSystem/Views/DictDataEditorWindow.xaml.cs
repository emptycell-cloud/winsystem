using System.Windows;
using System.Windows.Input;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    public partial class DictDataEditorWindow : Window
    {
        public DictDataEditorWindow()
        {
            InitializeComponent();
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
            if (DataContext is not DictDataEditorViewModel vm) return;
            if (vm.Validate()) { DialogResult = true; Close(); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}
