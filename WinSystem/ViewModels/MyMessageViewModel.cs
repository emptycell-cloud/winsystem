using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;
using WinSystem.Services;
using WinSystem.Views;

namespace WinSystem.ViewModels
{
    /// <summary>我的消息页 ViewModel（接收端）：当前用户可见消息的服务端分页列表 + 未读统计 + 标记已读。</summary>
    public class MyMessageViewModel : ViewModelBase
    {
        private readonly DataService _data;
        private string _searchTitle = "";
        private int _searchType;     // 0=全部 1通知 2公告 3提醒
        private bool _isLoading;
        private bool _dataLoaded;
        private MyMessageItem? _selected;
        private int _pageIndex = 1;
        private int _pageSize = 15;
        private int _totalCount;
        private int _totalPages = 1;
        private int _unreadCount;

        public MyMessageViewModel()
        {
            _data = Session.Data;
            ViewCommand = new RelayCommand(async o => await View(o as MyMessageItem));
            MarkReadCommand = new RelayCommand(async o => await MarkReadAsync(o as MyMessageItem));
            FirstPageCommand = new RelayCommand(_ => PageIndex = 1, _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(_ => PageIndex--, _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(_ => PageIndex++, _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(_ => PageIndex = TotalPages, _ => PageIndex < TotalPages);
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
        }

        public ObservableCollection<MyMessageItem> MyMessages { get; } = new();

        /// <summary>类型筛选下拉标签（索引即类型值：0全部 1通知 2公告 3提醒）。</summary>
        public string[] TypeLabels { get; } = { "全部", "通知", "公告", "提醒" };

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

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public MyMessageItem? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public int UnreadCount
        {
            get => _unreadCount;
            set => SetProperty(ref _unreadCount, value);
        }

        /// <summary>未读统计文本，如“未读 3 条”。</summary>
        public string UnreadText => UnreadCount > 0 ? $"未读 {UnreadCount} 条" : "暂无未读消息";

        public ICommand ViewCommand { get; }
        public ICommand MarkReadCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand SearchCommand { get; }

        // ================= 分页 =================

        public int[] PageSizeOptions { get; } = { 15, 30, 50, 100 };

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (SetProperty(ref _pageIndex, Math.Clamp(value, 1, Math.Max(TotalPages, 1))))
                {
                    _ = LoadCoreAsync();
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
                    _pageIndex = 1;
                    OnPropertyChanged(nameof(PageIndex));
                    _ = LoadCoreAsync();
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

        public string PageInfo => $"共 {TotalCount} 条 · 第 {PageIndex}/{TotalPages} 页";

        /// <summary>页面激活时调用：首次进入时加载一次。</summary>
        public Task ReloadAsync()
        {
            if (!_dataLoaded)
            {
                _dataLoaded = true;
                return LoadCoreAsync();
            }
            return Task.CompletedTask;
        }

        /// <summary>服务端分页加载当前页数据 + 未读统计。</summary>
        private async Task LoadCoreAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                var result = await _data.LoadMyMessagesAsync(PageIndex, PageSize, SearchTitle, SearchType);
                TotalCount = result.Total;
                TotalPages = Math.Max(1, (int)Math.Ceiling(result.Total / (double)PageSize));
                if (_pageIndex > TotalPages)
                {
                    _pageIndex = Math.Max(1, TotalPages);
                    OnPropertyChanged(nameof(PageIndex));
                    return; // 越界修正后重新加载
                }

                MyMessages.Clear();
                foreach (var m in result.Items) MyMessages.Add(m);
                UnreadCount = await _data.GetMyUnreadCountAsync();
                OnPropertyChanged(nameof(UnreadText));
                OnPropertyChanged(nameof(PageInfo));
            }
            catch (Exception ex)
            {
                DialogService.Error(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>点击“查询”按钮：回到第 1 页并按条件重新加载（加载动画层由 LoadCoreAsync 内的 IsLoading 控制）。</summary>
        private async Task SearchAsync()
        {
            if (IsLoading) return;
            if (_pageIndex != 1) PageIndex = 1;   // setter 触发 LoadCoreAsync 刷新
            else await LoadCoreAsync();           // 已在第 1 页则直接重新加载
        }

        /// <summary>查看详情（打开详情窗并自动标记已读）。</summary>
        private async Task View(MyMessageItem? item)
        {
            item ??= Selected;
            if (item == null) return;
            await MarkReadAsync(item);
            var detail = await _data.LoadMessageAsync(item.Id);
            if (detail != null) DialogService.ShowMessageDetail(new MessageDetailViewModel(detail));
        }

        /// <summary>标记单条为已读（行内按钮，仅未读显示）。</summary>
        private async Task MarkReadAsync(MyMessageItem? item)
        {
            if (item == null || item.IsRead) return;
            try
            {
                await _data.MarkMyMessageReadAsync(item.Id);
                item.IsRead = true;
                UnreadCount = Math.Max(0, UnreadCount - 1);
                OnPropertyChanged(nameof(UnreadText));
                MainViewModel.Current?.RefreshMyMessageUnreadBadge(); // 同步侧边栏未读气泡
            }
            catch (Exception ex)
            {
                DialogService.Error(ex.Message);
            }
        }
    }
}
