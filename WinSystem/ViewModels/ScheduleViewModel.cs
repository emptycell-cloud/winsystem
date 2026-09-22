using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>月历中的单个日期格子。</summary>
    public class ScheduleDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsSelected { get; set; }
        public string DayNumber => Date.Day.ToString();
        public string LunarText => Helpers.ChineseLunar.GetLunarText(Date);
        public List<SysSchedule> Schedules { get; set; } = new();

        /// <summary>格子内最多展示的日程数。</summary>
        public List<SysSchedule> VisibleSchedules => Schedules.Take(3).ToList();

        /// <summary>超出展示数量而折叠的日程数。</summary>
        public int MoreCount => Math.Max(0, Schedules.Count - 3);

        /// <summary>是否还有更多日程未展示。</summary>
        public bool HasMore => MoreCount > 0;
    }

    /// <summary>日程管理页 ViewModel：日程（sys_schedule）按月历展示与增删改查。</summary>
    public class ScheduleViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private DateTime _month = new(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime _selectedDate = DateTime.Today;
        private bool _dataLoaded;

        public ScheduleViewModel()
        {
            _data = Services.Session.Data;
            PrevMonthCommand = new RelayCommand(_ => Month = Month.AddMonths(-1));
            NextMonthCommand = new RelayCommand(_ => Month = Month.AddMonths(1));
            GoTodayCommand = new RelayCommand(_ => GoToday());
            SelectDateCommand = new RelayCommand(o =>
            {
                if (o is ScheduleDay day) SelectDate(day.Date);
            });
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as SysSchedule));
            DeleteCommand = new RelayCommand(async o => await Delete(o as SysSchedule));
        }

        public ObservableCollection<ScheduleDay> CalendarDays { get; } = new();
        public ObservableCollection<SysSchedule> DaySchedules { get; } = new();

        public ICommand PrevMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand GoTodayCommand { get; }
        public ICommand SelectDateCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        /// <summary>当前展示的月份（取每月 1 日）。</summary>
        public DateTime Month
        {
            get => _month;
            set
            {
                if (SetProperty(ref _month, new DateTime(value.Year, value.Month, 1)))
                    RebuildCalendar();
            }
        }

        public string MonthTitle => Month.ToString("yyyy年M月");

        /// <summary>选中日期标题，如“2026年9月15日 · 星期二”。</summary>
        public string SelectedDateTitle => $"{_selectedDate:yyyy年M月d日} · {WeekText(_selectedDate)}";

        /// <summary>页面激活时调用：日程首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (!_dataLoaded) await LoadCoreAsync();
        }

        private async Task LoadCoreAsync(bool refresh = false)
        {
            if (refresh || !_dataLoaded) await _data.LoadSchedulesAsync();
            _dataLoaded = true;
            RebuildCalendar();
        }

        private void GoToday()
        {
            Month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            SelectDate(DateTime.Today);
        }

        /// <summary>选中某一天：更新格子高亮并刷新下方明细。</summary>
        private void SelectDate(DateTime date)
        {
            _selectedDate = date.Date;
            foreach (var d in CalendarDays) d.IsSelected = d.Date == _selectedDate;
            RefreshDaySchedules();
        }

        /// <summary>构建当前月 42 格（6 周，周一起始），并填充各日日程。</summary>
        private void RebuildCalendar()
        {
            CalendarDays.Clear();
            var first = Month;
            var offset = ((int)first.DayOfWeek + 6) % 7; // 周一=0
            var start = first.AddDays(-offset);
            var today = DateTime.Today;
            for (var i = 0; i < 42; i++)
            {
                var date = start.AddDays(i);
                CalendarDays.Add(new ScheduleDay
                {
                    Date = date,
                    IsCurrentMonth = date.Year == Month.Year && date.Month == Month.Month,
                    IsToday = date.Date == today,
                    IsSelected = date.Date == _selectedDate.Date,
                    Schedules = _data.Schedules
                        .Where(s => s.Status == 1 && s.ScheduleDate.Date == date.Date)
                        .OrderBy(s => string.IsNullOrEmpty(s.StartTime)).ThenBy(s => s.StartTime)
                        .ToList()
                });
            }
            OnPropertyChanged(nameof(MonthTitle));
            RefreshDaySchedules();
        }

        /// <summary>刷新选中日期的日程明细列表。</summary>
        private void RefreshDaySchedules()
        {
            DaySchedules.Clear();
            var list = _data.Schedules
                .Where(s => s.Status == 1 && s.ScheduleDate.Date == _selectedDate.Date)
                .OrderBy(s => string.IsNullOrEmpty(s.StartTime)).ThenBy(s => s.StartTime);
            foreach (var s in list) DaySchedules.Add(s);
            OnPropertyChanged(nameof(SelectedDateTitle));
        }

        private static string WeekText(DateTime date) => date.DayOfWeek switch
        {
            DayOfWeek.Monday => "星期一",
            DayOfWeek.Tuesday => "星期二",
            DayOfWeek.Wednesday => "星期三",
            DayOfWeek.Thursday => "星期四",
            DayOfWeek.Friday => "星期五",
            DayOfWeek.Saturday => "星期六",
            _ => "星期日"
        };

        private async Task Add()
        {
            var editor = ScheduleEditorViewModel.CreateNew();
            editor.ScheduleDate = _selectedDate; // 新增日程预填选中日期
            if (Views.DialogService.ShowScheduleEditor(editor) is true)
            {
                var schedule = editor.BuildEntity();
                try
                {
                    await _data.AddScheduleAsync(schedule);
                    await LoadCoreAsync(refresh: true);
                    Views.DialogService.Success($"日程“{schedule.Title}”新增成功。");
                }
                catch (Exception ex) { Views.DialogService.Error(ex.Message); }
            }
        }

        private async Task Edit(SysSchedule? schedule)
        {
            if (schedule is not SysSchedule target) return;
            var editor = new ScheduleEditorViewModel(target);
            if (Views.DialogService.ShowScheduleEditor(editor) is true)
            {
                editor.ApplyTo(target);
                try
                {
                    await _data.UpdateScheduleAsync(target);
                    SelectDate(target.ScheduleDate); // 若改了日期，跟随跳转
                    await LoadCoreAsync(refresh: true);
                    Views.DialogService.Success($"日程“{target.Title}”修改成功。");
                }
                catch (Exception ex) { Views.DialogService.Error(ex.Message); }
            }
        }

        private async Task Delete(SysSchedule? schedule)
        {
            if (schedule is not SysSchedule target) return;
            if (!Views.DialogService.Confirm($"确定要删除日程“{target.Title}”（{target.DateText}）吗？"))
                return;
            try
            {
                await _data.DeleteScheduleAsync(target);
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"日程“{target.Title}”已删除。");
            }
            catch (Exception ex) { Views.DialogService.Error(ex.Message); }
        }
    }
}
