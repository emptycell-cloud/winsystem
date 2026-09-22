using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private const int DefaultPageSize = 15;
        private readonly Services.DataService _data;
        private string _searchName = "";
        private string _searchUsername = "";
        private bool _isLoading;
        private User? _selected;
        private int _pageIndex = 1;
        private int _pageSize = DefaultPageSize;
        private int _totalCount;
        private int _totalPages = 1;

        public UserViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as User));
            DeleteCommand = new RelayCommand(async o => await Delete(o as User));
            ResetPasswordCommand = new RelayCommand(async o => await ResetPassword(o as User));
            FirstPageCommand = new RelayCommand(async _ => await GoPageAsync(1), _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(async _ => await GoPageAsync(PageIndex - 1), _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(async _ => await GoPageAsync(PageIndex + 1), _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(async _ => await GoPageAsync(TotalPages), _ => PageIndex < TotalPages);
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
            Refresh();
        }

        public ObservableCollection<User> Items { get; } = new();

        public string SearchName
        {
            get => _searchName;
            set { SetProperty(ref _searchName, value); }
        }

        public string SearchUsername
        {
            get => _searchUsername;
            set { SetProperty(ref _searchUsername, value); }
        }

        /// <summary>是否正在查询（用于显示加载动画层）。</summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public User? Selected
        {
            get => _selected;
            set { SetProperty(ref _selected, value); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand SearchCommand { get; }

        // ================= 分页 =================

        /// <summary>每页条数选项（最小值为首次默认选中值）。</summary>
        public int[] PageSizeOptions { get; } = { 15, 30, 50, 100 };

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (SetProperty(ref _pageIndex, Math.Clamp(value, 1, Math.Max(TotalPages, 1))))
                {
                    Refresh();
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    PageIndex = 1;
                    Refresh();
                }
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        /// <summary>分页信息文本，如“共 103 条 · 第 2/7 页”。</summary>
        public string PageInfo => $"共 {TotalCount} 条 · 第 {PageIndex}/{TotalPages} 页";

        /// <summary>点击“查询”按钮：弹出加载动画层并重新过滤数据。</summary>
        private async Task SearchAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                await Task.Delay(30);   // 让加载动画层先渲染出来
                if (PageIndex != 1) PageIndex = 1; // 触发刷新
                else Refresh();
                await Task.Delay(250);  // 保证加载动画可感知
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>翻页：显示加载动画层并切换到指定页。</summary>
        private async Task GoPageAsync(int page)
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                await Task.Delay(30);   // 让加载动画层先渲染出来
                PageIndex = page;       // 触发刷新
                await Task.Delay(250);  // 保证加载动画可感知
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>从 API 实时重新加载数据并刷新列表。</summary>
        public async Task ReloadAsync()
        {
            await _data.LoadUsersAsync();
            Refresh();
        }

        private void Refresh()
        {
            var name = _searchName?.Trim().ToLower() ?? "";
            var username = _searchUsername?.Trim().ToLower() ?? "";
            var all = _data.Users.Where(u =>
                (string.IsNullOrEmpty(name) || (u.Name ?? "").ToLower().Contains(name)) &&
                (string.IsNullOrEmpty(username) || (u.Username ?? "").ToLower().Contains(username))).ToList();
            all = all.OrderBy(u => u.Id).ToList();

            TotalCount = all.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)PageSize));
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Items.Clear();
            var start = (PageIndex - 1) * PageSize;
            var i = 0;
            foreach (var u in all.Skip(start).Take(PageSize))
            {
                u.Index = start + ++i;
                Items.Add(u);
            }

            OnPropertyChanged(nameof(PageInfo));
        }

        private async Task Add()
        {
            await _data.LoadRolesAsync(); // 确保角色下拉数据就绪
            var editor = UserEditorViewModel.CreateNew(_data.Users.Select(u => u.Username).ToList(), _data.Roles.ToList());
            if (Views.DialogService.ShowUserEditor(editor) is true)
            {
                var user = editor.BuildEntity();
                await _data.AddUserAsync(user);
                Refresh();
                Views.DialogService.Success($"用户“{user.Name}”新增成功。");
            }
        }

        private async Task Edit(User? user)
        {
            user ??= Selected;
            if (user == null) return;
            await _data.LoadRolesAsync(); // 确保角色下拉数据就绪
            var roleIds = await _data.LoadUserRoleIdsAsync(user.Id);
            var editor = new UserEditorViewModel(user, null, _data.Roles.ToList(), roleIds.FirstOrDefault());
            if (Views.DialogService.ShowUserEditor(editor) is true)
            {
                editor.ApplyTo(user);
                await _data.UpdateUserAsync(user);
                Refresh();
                Views.DialogService.Success($"用户“{user.Name}”修改成功。");
            }
        }

        private async Task Delete(User? user)
        {
            user ??= Selected;
            if (user is not User target) return;
            if (Services.Session.CurrentUser?.Id == target.Id)
            {
                Views.DialogService.Info("不能删除当前登录的账号。");
                return;
            }
            if (Views.DialogService.Confirm($"确定要删除用户“{target.Name}”吗？此操作不可恢复。"))
            {
                await _data.DeleteUserAsync(target);
                Refresh();
            }
        }

        private async Task ResetPassword(User? user)
        {
            user ??= Selected;
            if (user is not User target) return;
            if (!Views.DialogService.Confirm($"确定要将用户“{target.Name}”的密码重置为 123456 吗？"))
                return;
            await _data.ResetPasswordAsync(target);
            Views.DialogService.Success($"用户“{target.Name}”的密码已重置为 123456。");
        }
    }
}