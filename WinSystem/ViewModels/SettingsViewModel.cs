using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Services;

namespace WinSystem.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _oldPwd = "";
        private string _newPwd = "";
        private string _confirmPwd = "";
        private string _profileMsg = "";
        private string _pwdMsg = "";

        public SettingsViewModel()
        {
            SaveProfileCommand = new RelayCommand(async _ => await SaveProfile());
            ChangePasswordCommand = new RelayCommand(async _ => await ChangePassword());
            var u = Session.CurrentUser;
            Name = u?.Name ?? "";
            Username = u?.Username ?? "";
            Email = u?.Email ?? "";
            Phone = u?.Phone ?? "";
            Position = u?.Position ?? "";
        }

        public string Name { get; set; } = "";
        public string Username { get; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Position { get; } = "";
        public string AppVersion => "WinSystem 综合管理系统  v1.0";

        public ICommand SaveProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public string ProfileMsg
        {
            get => _profileMsg;
            set => SetProperty(ref _profileMsg, value);
        }
        public string PwdMsg
        {
            get => _pwdMsg;
            set => SetProperty(ref _pwdMsg, value);
        }

        public string OldPwd { get => _oldPwd; set => SetProperty(ref _oldPwd, value); }
        public string NewPwd { get => _newPwd; set => SetProperty(ref _newPwd, value); }
        public string ConfirmPwd { get => _confirmPwd; set => SetProperty(ref _confirmPwd, value); }

        private async Task SaveProfile()
        {
            var user = Session.CurrentUser;
            if (user == null) return;
            user.Name = Name.Trim();
            user.Email = Email.Trim();
            user.Phone = Phone.Trim();
            await Session.Data.UpdateUserAsync(user);
            ProfileMsg = "资料已保存。";
            Views.DialogService.Success("个人资料已保存。");
        }

        private async Task ChangePassword()
        {
            var user = Session.CurrentUser;
            if (user == null) return;
            if (NewPwd.Length < 8 || NewPwd.Length > 64 || !NewPwd.Any(char.IsLetter) || !NewPwd.Any(char.IsDigit))
            { PwdMsg = "新密码须为 8-64 位，且同时包含字母和数字。"; Views.DialogService.Info("新密码须为 8-64 位，且同时包含字母和数字。"); return; }
            if (NewPwd != ConfirmPwd) { PwdMsg = "两次输入的新密码不一致。"; Views.DialogService.Info("两次输入的新密码不一致。"); return; }

            var (success, message) = await Session.Data.ChangePasswordAsync(user.Id, OldPwd, NewPwd);
            if (!success) { PwdMsg = message; Views.DialogService.Error(message); return; }

            PwdMsg = message;
            OldPwd = NewPwd = ConfirmPwd = "";
            Views.DialogService.Success(message);
        }
    }
}