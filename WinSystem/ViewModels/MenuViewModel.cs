using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>菜单配置页 ViewModel：左侧树形目录 + 右侧显示选中菜单的详细信息 + 新增/编辑/删除。</summary>
    public class MenuViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private Menu? _selectedMenu;
        private bool _dataLoaded;

        public MenuViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditSelectedCommand = new RelayCommand(async _ => await Edit(SelectedMenu));
            DeleteSelectedCommand = new RelayCommand(async _ => await Delete(SelectedMenu), _ => SelectedMenu != null);
        }

        /// <summary>全部菜单扁平集合（供编辑器上级选择）。</summary>
        public ObservableCollection<Menu> AllMenus => _data.AllMenus;

        /// <summary>菜单树（根节点，Children 递归，层级目录/页面/按钮），供左侧树形目录绑定。</summary>
        public ObservableCollection<Menu> MenuTree { get; } = new();

        /// <summary>当前选中的菜单，右侧详情绑定。</summary>
        public Menu? SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                if (SetProperty(ref _selectedMenu, value))
                {
                    if (value != null) value.ParentName = ResolveParentName(value);
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditSelectedCommand { get; }
        public ICommand DeleteSelectedCommand { get; }

        /// <summary>从扁平列表解析上级菜单名称（按 ParentId 反查）；根级显示兜底文本。</summary>
        private string ResolveParentName(Menu m)
        {
            if (string.IsNullOrEmpty(m.ParentId)) return "根级";
            var parent = _data.AllMenus.FirstOrDefault(x => x.Id == m.ParentId);
            return string.IsNullOrEmpty(parent?.Name) ? "" : parent.Name!;
        }

        /// <summary>供页面激活时调用：左侧菜单树仅在首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (_dataLoaded) return;
            await LoadCoreAsync();
        }

        /// <summary>组装树并在数据变化后刷新；refresh=true 时强制重新拉取扁平列表。</summary>
        private async Task LoadCoreAsync(bool refresh = false)
        {
            await _data.LoadAllMenusAsync(force: refresh); // 扁平列表（含 type=3），已登录预载
            _dataLoaded = true;
            BuildTree();

            if (SelectedMenu != null)
            {
                SelectedMenu.ParentName = ResolveParentName(SelectedMenu);
                OnPropertyChanged(nameof(SelectedMenu));
            }
            OnPropertyChanged(nameof(MenuTree));
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>依据 AllMenus 的 ParentId 组装层级树（根 = ParentId 为空）。</summary>
        private void BuildTree()
        {
            MenuTree.Clear();
            var nodes = _data.AllMenus.ToDictionary(m => m.Id);
            foreach (var menu in _data.AllMenus)
            {
                menu.Children.Clear();
            }
            foreach (var menu in _data.AllMenus)
            {
                if (!string.IsNullOrEmpty(menu.ParentId) && nodes.TryGetValue(menu.ParentId, out var parent))
                    parent.Children.Add(menu);
                else
                    MenuTree.Add(menu);
            }
        }

        private async Task Add()
        {
            var editor = MenuEditorViewModel.CreateNew(_data.AllMenus.ToList());
            if (Views.DialogService.ShowMenuEditor(editor) is true)
            {
                var menu = editor.BuildEntity();
                await _data.AddMenuAsync(menu);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"菜单“{menu.Name}”新增成功。");
            }
        }

        private async Task Edit(Menu? menu)
        {
            if (menu == null) return;
            var editor = new MenuEditorViewModel(menu, _data.AllMenus.ToList());
            if (Views.DialogService.ShowMenuEditor(editor) is true)
            {
                editor.ApplyTo(menu);
                await _data.UpdateMenuAsync(menu);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"菜单“{menu.Name}”修改成功。");
            }
        }

        private async Task Delete(Menu? menu)
        {
            if (menu is not Menu target) return;
            if (!Views.DialogService.Confirm($"确定要删除菜单“{target.Name}”吗？"))
                return;
            try
            {
                await _data.DeleteMenuAsync(target);
                SelectedMenu = null;
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"菜单“{target.Name}”已删除。");
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }
    }
}