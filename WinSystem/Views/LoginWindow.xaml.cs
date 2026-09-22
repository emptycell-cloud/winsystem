using System.Windows;
using System.Windows.Input;
using WinSystem.Services;

namespace WinSystem.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryLogin();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Login_Click(object sender, RoutedEventArgs e) => TryLogin();

        private async void TryLogin()
        {
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "请输入账号和密码。";
                return;
            }

            try
            {
                LoginBtn.IsEnabled = false;
                LoginBtn.Content = "登录中…";

                var user = await Session.Data.LoginAsync(username, password);
                if (user == null) { ErrorText.Text = "账号或密码错误。"; return; }
                if (user.Status == 2) { ErrorText.Text = "该账号已被禁用，请联系管理员。"; return; }
                if (user.Status == 3) { ErrorText.Text = "该账号待激活，请联系管理员。"; return; }

                Session.CurrentUser = user;
                await Session.Data.LoadAsync();

                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception)
            {
                ErrorText.Text = "无法连接服务器，请确认 API 服务已启动。";
            }
            finally
            {
                LoginBtn.IsEnabled = true;
                LoginBtn.Content = "登 录";
            }
        }
    }
}