using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>消息通知页 ViewModel：消息列表 + 新增/编辑/发布/删除/查看详情（数据来自 sys_message）。</summary>
    public class MessageViewModel : ViewModelBase
    {
        private const int DefaultPageSize = 15;
        private readonly Services.DataService _data;
        private string _searchTitle = "";
        private int _searchType;     // 0=全部 1通知 2公告 3提醒
        private int _searchStatus;   // 0=全部 1已发布 2草稿
        private bool _isLoading;
        private bool _dataLoaded;
        private SysMessage? _selected;
        private int _pageIndex = 1;
        private int _pageSize = DefaultPageSize;
        private int _totalCount;
        private int _totalPages = 1;

        public MessageViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as SysMessage));
            ViewCommand = new RelayCommand(async o => await View(o as SysMessage));
            PublishCommand = new RelayCommand(async o => await Publish(o as SysMessage));
            DeleteCommand = new RelayCommand(async o => await Delete(o as SysMessage));
            FirstPageCommand = new RelayCommand(_ => PageIndex = 1, _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(_ => PageIndex--, _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(_ => PageIndex++, _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(_ => PageIndex = TotalPages, _ => PageIndex < TotalPages);
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
            Refresh();
        }

        public ObservableCollection<SysMessage> Items { get; } = new();

        /// <summary>类型筛选下拉标签（索引即类型值：0全部 1通知 2公告 3提醒）。</summary>
        public string[] TypeLabels { get; } = { "全部", "通知", "公告", "提醒" };

        /// <summary>状态筛选下拉标签（索引即状态值：0全部 1已发布 2草稿）。</summary>
        public string[] StatusLabels { get; } = { "全部", "已发布", "草稿" };

        public string SearchTitle
        {
            get => _searchTitle;
            set => SetProperty(ref _searchTitle, value);
        }

        public int SearchType
        {
            get => _searchType;
            set => SetProperty(ref _searchType, value);
        }

        public int SearchStatus
        {
            get => _searchStatus;
            set => SetProperty(ref _searchStatus, value);
        }

        /// <summary>是否正在查询（用于显示加载动画层）。</summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public SysMessage? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ViewCommand { get; }
        public ICommand PublishCommand { get; }
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

        /// <summary>页面激活时调用：首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (_dataLoaded) return;
            await LoadCoreAsync();
        }

        private async Task LoadCoreAsync(bool refresh = false)
        {
            if (refresh || !_dataLoaded) await _data.LoadMessagesAsync();
            _dataLoaded = true;
            Refresh();
        }

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

        private void Refresh()
        {
            var title = _searchTitle?.Trim().ToLower() ?? "";
            var all = _data.Messages.Where(m =>
                (string.IsNullOrEmpty(title) || (m.Title ?? "").ToLower().Contains(title)) &&
                (_searchType == 0 || m.Type == _searchType) &&
                (_searchStatus == 0 || m.Status == _searchStatus)).ToList();
            all = all.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id).ToList();

            TotalCount = all.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)PageSize));
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Items.Clear();
            var page = all.Skip((PageIndex - 1) * PageSize).Take(PageSize);
            foreach (var m in page) Items.Add(m);

            OnPropertyChanged(nameof(PageInfo));
        }

        private async Task Add()
        {
            await PrepareEditorDataAsync();
            var editor = MessageEditorViewModel.CreateNew(_data);
            if (Views.DialogService.ShowMessageEditor(editor) is true)
            {
                var message = editor.BuildEntity();
                await _data.AddMessageAsync(message);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"消息“{message.Title}”新增成功。");
            }
        }

        private async Task Edit(SysMessage? message)
        {
            message ??= Selected;
            if (message == null) return;
            await PrepareEditorDataAsync();
            var detail = await _data.LoadMessageAsync(message.Id) ?? message;
            var editor = new MessageEditorViewModel(_data, detail);
            if (Views.DialogService.ShowMessageEditor(editor) is true)
            {
                editor.ApplyTo(message);
                await _data.UpdateMessageAsync(message);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"消息“{message.Title}”修改成功。");
            }
        }

        private async Task View(SysMessage? message)
        {
            message ??= Selected;
            if (message == null) return;
            var detail = await _data.LoadMessageAsync(message.Id) ?? message;
            Views.DialogService.ShowMessageDetail(new MessageDetailViewModel(detail));
        }

        private async Task Publish(SysMessage? message)
        {
            message ??= Selected;
            if (message is not SysMessage target) return;
            if (target.Status == 1)
            {
                Views.DialogService.Info("该消息已发布。");
                return;
            }
            if (!Views.DialogService.Confirm($"确定要发布消息“{target.Title}”吗？"))
                return;
            await _data.PublishMessageAsync(target);
            await LoadCoreAsync(refresh: true);
            Views.DialogService.Success($"消息“{target.Title}”已发布。");
        }

        private async Task Delete(SysMessage? message)
        {
            message ??= Selected;
            if (message is not SysMessage target) return;
            if (!Views.DialogService.Confirm($"确定要删除消息“{target.Title}”吗？此操作不可恢复。"))
                return;
            try
            {
                await _data.DeleteMessageAsync(target);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"消息“{target.Title}”已删除。");
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }

        /// <summary>确保编辑窗目标下拉所需的角色/组织/用户数据已加载。</summary>
        private async Task PrepareEditorDataAsync()
        {
            await _data.LoadRolesAsync();
            await _data.LoadOrganizationsAsync();
            await _data.LoadAllOrganizationsAsync();
            if (_data.Users.Count == 0) await _data.LoadUsersAsync();
        }
    }
}
