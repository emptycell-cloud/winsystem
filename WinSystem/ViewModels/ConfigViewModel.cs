using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>系统参数页 ViewModel：系统参数（sys_config）列表的增删改查。</summary>
    public class ConfigViewModel : ViewModelBase
    {
        private const int DefaultPageSize = 15;
        private readonly Services.DataService _data;
        private string _searchText = "";
        private bool _dataLoaded;
        private int _pageIndex = 1;
        private int _pageSize = DefaultPageSize;
        private int _totalCount;
        private int _totalPages = 1;

        public ConfigViewModel()
        {
            _data = Services.Session.Data;
            SearchCommand = new RelayCommand(_ => Refresh());
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as SysConfig));
            DeleteCommand = new RelayCommand(async o => await Delete(o as SysConfig));
            FirstPageCommand = new RelayCommand(_ => PageIndex = 1, _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(_ => PageIndex--, _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(_ => PageIndex++, _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(_ => PageIndex = TotalPages, _ => PageIndex < TotalPages);
        }

        public ObservableCollection<SysConfig> Items { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

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

        /// <summary>页面激活时调用：系统参数仅首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (!_dataLoaded) await LoadCoreAsync();
        }

        private async Task LoadCoreAsync(bool refresh = false)
        {
            if (refresh || !_dataLoaded) await _data.LoadConfigsAsync();
            _dataLoaded = true;
            Refresh();
        }

        /// <summary>按查询关键字过滤后分页，重建当前页列表。</summary>
        private void Refresh()
        {
            var kw = _searchText?.Trim().ToLower() ?? "";
            var all = string.IsNullOrEmpty(kw)
                ? _data.Configs.ToList()
                : _data.Configs.Where(c =>
                      (c.ConfigName ?? "").ToLower().Contains(kw) ||
                      (c.ConfigKey ?? "").ToLower().Contains(kw)).ToList();

            TotalCount = all.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)PageSize));
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Items.Clear();
            var page = all.Skip((PageIndex - 1) * PageSize).Take(PageSize);
            foreach (var c in page) Items.Add(c);

            OnPropertyChanged(nameof(PageInfo));
        }

        private async Task Add()
        {
            var editor = ConfigEditorViewModel.CreateNew();
            if (Views.DialogService.ShowConfigEditor(editor) is true)
            {
                var cfg = editor.BuildEntity();
                try
                {
                    await _data.AddConfigAsync(cfg);
                    await LoadCoreAsync(refresh: true);
                    Views.DialogService.Success($"系统参数“{cfg.ConfigName}”新增成功。");
                }
                catch (Exception ex) { Views.DialogService.Error(ex.Message); }
            }
        }

        private async Task Edit(SysConfig? cfg)
        {
            if (cfg is not SysConfig target) return;
            var editor = new ConfigEditorViewModel(target);
            if (Views.DialogService.ShowConfigEditor(editor) is true)
            {
                editor.ApplyTo(target);
                try
                {
                    await _data.UpdateConfigAsync(target);
                    await LoadCoreAsync(refresh: true);
                    Views.DialogService.Success($"系统参数“{target.ConfigName}”修改成功。");
                }
                catch (Exception ex) { Views.DialogService.Error(ex.Message); }
            }
        }

        private async Task Delete(SysConfig? cfg)
        {
            if (cfg is not SysConfig target) return;
            if (!Views.DialogService.Confirm($"确定要删除系统参数“{target.ConfigName}”（{target.ConfigKey}）吗？"))
                return;
            try
            {
                await _data.DeleteConfigAsync(target);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"系统参数“{target.ConfigName}”已删除。");
            }
            catch (Exception ex) { Views.DialogService.Error(ex.Message); }
        }
    }
}
