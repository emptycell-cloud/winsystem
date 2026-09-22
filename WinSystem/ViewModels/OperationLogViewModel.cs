using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>操作日志页 ViewModel：操作日志（sys_operation_log）的查询、查看、删除与清空。</summary>
    public class OperationLogViewModel : ViewModelBase
    {
        private const int DefaultPageSize = 15;
        private readonly Services.DataService _data;
        private string _searchKeyword = "";
        private int _searchAction = 0;
        private int _searchStatus = 0;
        private bool _isLoading;
        private SysOperationLog? _selected;
        private int _pageIndex = 1;
        private int _pageSize = DefaultPageSize;
        private int _totalCount;
        private int _totalPages = 1;

        public OperationLogViewModel()
        {
            _data = Services.Session.Data;
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
            ViewCommand = new RelayCommand(o => View(o as SysOperationLog));
            DeleteCommand = new RelayCommand(async o => await Delete(o as SysOperationLog));
            ClearCommand = new RelayCommand(async _ => await ClearAllAsync());
            FirstPageCommand = new RelayCommand(async _ => await GoPageAsync(1), _ => PageIndex > 1);
            PrevPageCommand = new RelayCommand(async _ => await GoPageAsync(PageIndex - 1), _ => PageIndex > 1);
            NextPageCommand = new RelayCommand(async _ => await GoPageAsync(PageIndex + 1), _ => PageIndex < TotalPages);
            LastPageCommand = new RelayCommand(async _ => await GoPageAsync(TotalPages), _ => PageIndex < TotalPages);
        }

        public ObservableCollection<SysOperationLog> Items { get; } = new();

        /// <summary>搜索关键字：匹配操作人、模块、路径、详情。</summary>
        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        /// <summary>操作类型筛选下拉选项（全部+各操作）。</summary>
        public string[] ActionOptions { get; } = { "全部操作", "新增", "编辑", "删除", "登录" };

        /// <summary>操作类型筛选索引。</summary>
        public int SearchAction
        {
            get => _searchAction;
            set => SetProperty(ref _searchAction, value);
        }

        /// <summary>执行状态筛选下拉选项。</summary>
        public string[] StatusOptions { get; } = { "全部状态", "成功", "失败" };

        /// <summary>执行状态筛选索引。</summary>
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

        public SysOperationLog? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand ViewCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
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

        /// <summary>页面激活时调用：操作日志仅首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            await _data.LoadOperationLogsAsync();
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

        /// <summary>按关键字/操作类型/状态过滤后分页，重建当前页列表。</summary>
        private void Refresh()
        {
            var kw = _searchKeyword?.Trim().ToLower() ?? "";
            var action = _searchAction >= 1 && _searchAction < ActionOptions.Length ? ActionOptions[_searchAction] : null;
            var status = _searchStatus >= 1 && _searchStatus < StatusOptions.Length ? StatusOptions[_searchStatus] : null;

            var all = _data.OperationLogs.Where(l =>
                (string.IsNullOrEmpty(kw) ||
                 (l.Username ?? "").ToLower().Contains(kw) ||
                 (l.Name ?? "").ToLower().Contains(kw) ||
                 (l.Module ?? "").ToLower().Contains(kw) ||
                 (l.Path ?? "").ToLower().Contains(kw) ||
                 (l.Detail ?? "").ToLower().Contains(kw)) &&
                (action == null || string.Equals(l.Action, action, StringComparison.Ordinal)) &&
                (status == null || string.Equals(l.StatusText, status, StringComparison.Ordinal))).ToList();

            TotalCount = all.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)PageSize));
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Items.Clear();
            var start = (PageIndex - 1) * PageSize;
            var i = 0;
            foreach (var l in all.Skip(start).Take(PageSize))
            {
                l.Index = start + ++i;
                Items.Add(l);
            }

            OnPropertyChanged(nameof(PageInfo));
        }

        /// <summary>查看日志详情。</summary>
        private void View(SysOperationLog? log)
        {
            log ??= Selected;
            if (log is not SysOperationLog target) return;
            Views.DialogService.ShowOperationLogDetail(target);
        }

        /// <summary>删除单条日志。</summary>
        private async Task Delete(SysOperationLog? log)
        {
            log ??= Selected;
            if (log is not SysOperationLog target) return;
            if (!Views.DialogService.Confirm($"确定要删除 {target.CreatedText} 的这条操作日志吗？"))
                return;
            try
            {
                await _data.DeleteOperationLogAsync(target);
                Refresh();
                Views.DialogService.Success("操作日志已删除。");
            }
            catch (Exception ex) { Views.DialogService.Error(ex.Message); }
        }

        /// <summary>清空全部日志。</summary>
        private async Task ClearAllAsync()
        {
            if (_data.OperationLogs.Count == 0)
            {
                Views.DialogService.Info("当前没有操作日志。");
                return;
            }
            if (!Views.DialogService.Confirm($"确定要清空全部 {_data.OperationLogs.Count} 条操作日志吗？此操作不可恢复。"))
                return;
            try
            {
                await _data.ClearOperationLogsAsync();
                Refresh();
                Views.DialogService.Success("操作日志已清空。");
            }
            catch (Exception ex) { Views.DialogService.Error(ex.Message); }
        }
    }
}
