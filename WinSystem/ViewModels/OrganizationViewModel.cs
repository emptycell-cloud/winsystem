using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>组织机构页 ViewModel：左侧树形目录 + 右侧显示选中组织的详细信息 + 新增/编辑/删除。</summary>
    public class OrganizationViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private Organization? _selectedOrganization;
        private bool _dataLoaded;

        public OrganizationViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditSelectedCommand = new RelayCommand(async _ => await Edit(SelectedOrganization));
            DeleteSelectedCommand = new RelayCommand(async _ => await Delete(SelectedOrganization), _ => SelectedOrganization != null);
        }

        /// <summary>组织树（根节点列表，Children 递归），供左侧树形目录绑定。</summary>
        public ObservableCollection<Organization> Organizations => _data.Organizations;

        /// <summary>当前选中的组织，右侧详情绑定。</summary>
        public Organization? SelectedOrganization
        {
            get => _selectedOrganization;
            set
            {
                if (SetProperty(ref _selectedOrganization, value))
                {
                    // 填充上级名称（树数据不含 ParentName）；根级显示兜底文本
                    if (value != null) value.ParentName = ResolveParentName(value);
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>解析组织上级名称：从实时加载的扁平列表按 ParentId 反查；根级显示兜底文本。</summary>
        private string ResolveParentName(Organization org)
        {
            if (string.IsNullOrEmpty(org.ParentId)) return "根级（集团总部）";
            var parent = _data.AllOrganizations.FirstOrDefault(o => o.Id == org.ParentId);
            return string.IsNullOrEmpty(parent?.Name) ? "" : parent.Name!;
        }

        public ICommand AddCommand { get; }
        public ICommand EditSelectedCommand { get; }
        public ICommand DeleteSelectedCommand { get; }

        /// <summary>供页面激活时调用：左侧组织树仅在首次进入时加载一次，避免反复拉取。</summary>
        public async Task ReloadAsync()
        {
            if (_dataLoaded) return;
            await LoadCoreAsync();
        }

        /// <summary>真正加载组织树与名称字典；数据发生增删改后也调用以保持最新。</summary>
        private async Task LoadCoreAsync(bool refreshTree = false)
        {
            await _data.LoadAllOrganizationsAsync(force: refreshTree); // 扁平列表（父级名称），增删改后强制刷新
            await _data.LoadOrganizationsAsync(force: refreshTree); // 增删改后强制刷新树
            _dataLoaded = true;

            // 重新解析选中项的上级名称并强制刷新右侧（确保切入时数据一致）
            if (SelectedOrganization != null)
            {
                SelectedOrganization.ParentName = ResolveParentName(SelectedOrganization);
                OnPropertyChanged(nameof(SelectedOrganization));
            }
            OnPropertyChanged(nameof(Organizations));
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private async Task Add()
        {
            var editor = OrganizationEditorViewModel.CreateNew(_data.AllOrganizations.ToList());
            if (Views.DialogService.ShowOrganizationEditor(editor) is true)
            {
                var org = editor.BuildEntity();
                await _data.AddOrganizationAsync(org);
                await LoadCoreAsync(refreshTree: true);
                Views.DialogService.Success($"组织“{org.Name}”新增成功。");
            }
        }

        private async Task Edit(Organization? org)
        {
            if (org == null) return;
            var editor = new OrganizationEditorViewModel(org, _data.AllOrganizations.ToList());
            if (Views.DialogService.ShowOrganizationEditor(editor) is true)
            {
                editor.ApplyTo(org);
                await _data.UpdateOrganizationAsync(org);
                await LoadCoreAsync(refreshTree: true);
                Views.DialogService.Success($"组织“{org.Name}”修改成功。");
            }
        }

        private async Task Delete(Organization? org)
        {
            if (org is not Organization target) return;
            if (!Views.DialogService.Confirm($"确定要删除组织“{target.Name}”吗？"))
                return;
            try
            {
                await _data.DeleteOrganizationAsync(target);
                SelectedOrganization = null;
                await LoadCoreAsync(refreshTree: true);
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }
    }
}