using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>角色权限页 ViewModel：角色列表 + 新增/编辑/分配权限/删除（数据来自 sys_role）。</summary>
    public class RoleViewModel : ViewModelBase
    {
        private const int DefaultPageSize = 15;
        private readonly Services.DataService _data;
        private string _searchName = "";
        private Role? _selected;
        private bool _dataLoaded;
        private int _pageIndex = 1;
        private int _pageSize = DefaultPageSize;
        private int _totalCount;
        private int _totalPages = 1;

        public RoleViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as Role));
            PermissionCommand = new RelayCommand(async o => await AssignPermission(o as Role));
            DeleteCommand = new RelayCommand(async o => await Delete(o as Role));
            FirstPageCommand = new RelayCommand(_ => PageIndex = 1, _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(_ => PageIndex--, _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(_ => PageIndex++, _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(_ => PageIndex = TotalPages, _ => PageIndex < TotalPages);
            SearchCommand = new RelayCommand(_ => { PageIndex = 1; Refresh(); });
            Refresh();
        }

        public ObservableCollection<Role> Items { get; } = new();

        public string SearchName
        {
            get => _searchName;
            set { SetProperty(ref _searchName, value); }
        }

        public Role? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand PermissionCommand { get; }
        public ICommand DeleteCommand { get; }
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

        /// <summary>供页面激活时调用：角色列表仅首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (_dataLoaded) return;
            await LoadCoreAsync();
        }

        private async Task LoadCoreAsync(bool refresh = false)
        {
            if (refresh || !_dataLoaded) await _data.LoadRolesAsync();
            _dataLoaded = true;
            Refresh();
        }

        private void Refresh()
        {
            var name = _searchName?.Trim().ToLower() ?? "";
            var all = string.IsNullOrEmpty(name)
                ? _data.Roles.ToList()
                : _data.Roles.Where(r => (r.Name ?? "").ToLower().Contains(name)).ToList();

            TotalCount = all.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)PageSize));
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Items.Clear();
            var page = all.Skip((PageIndex - 1) * PageSize).Take(PageSize);
            foreach (var r in page) Items.Add(r);

            OnPropertyChanged(nameof(PageInfo));
        }

        private async Task Add()
        {
            var editor = RoleEditorViewModel.CreateNew();
            if (Views.DialogService.ShowRoleEditor(editor) is true)
            {
                var role = editor.BuildEntity();
                await _data.AddRoleAsync(role);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"角色“{role.Name}”新增成功。");
            }
        }

        private async Task Edit(Role? role)
        {
            role ??= Selected;
            if (role == null) return;
            var editor = new RoleEditorViewModel(role);
            if (Views.DialogService.ShowRoleEditor(editor) is true)
            {
                editor.ApplyTo(role);
                await _data.UpdateRoleAsync(role);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"角色“{role.Name}”修改成功。");
            }
        }

        private async Task AssignPermission(Role? role)
        {
            role ??= Selected;
            if (role == null) return;
            var assigned = await _data.LoadRolePermissionsAsync(role.Id);
            var vm = new RolePermissionViewModel(role, assigned);
            if (Views.DialogService.ShowRolePermission(vm) is true)
            {
                await _data.SaveRolePermissionsAsync(role.Id, vm.SelectedPermissions);
                Views.DialogService.Success($"角色“{role.Name}”权限已保存。");
            }
        }

        private async Task Delete(Role? role)
        {
            role ??= Selected;
            if (role is not Role target) return;
            if (target.IsBuiltIn == 1)
            {
                Views.DialogService.Info("内置角色不可删除。");
                return;
            }
            if (!Views.DialogService.Confirm($"确定要删除角色“{target.Name}”吗？"))
                return;
            try
            {
                await _data.DeleteRoleAsync(target);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"角色“{target.Name}”已删除。");
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }
    }
}