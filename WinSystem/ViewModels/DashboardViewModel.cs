using System.Collections.ObjectModel;
using WinSystem.Commands;
using WinSystem.Models;
using WinSystem.Services;

namespace WinSystem.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly DataService _data;
        private string _scheduleSummary = "";

        /// <summary>由 MainViewModel 注入：点击日程卡片中的某条日程时跳转到日程管理模块并定位。</summary>
        public Action<SysSchedule>? OpenSchedule { get; set; }

        public DashboardViewModel()
        {
            _data = Session.Data;
            OpenScheduleCommand = new RelayCommand(o => { if (o is SysSchedule s) OpenSchedule?.Invoke(s); });
            _ = ReloadAsync();
        }

        /// <summary>点击「近 3 天日程」卡片，跳转日程管理模块。</summary>
        public RelayCommand OpenScheduleCommand { get; }

        public string Greeting { get; } =
            $"{TimeGreeting()}，{Session.CurrentUser?.Name ?? "管理员"}";
        public string GreetingSub => $"今天是 {DateTime.Now:yyyy年M月d日 dddd}，祝您工作顺利！";

        /// <summary>近 3 天（含今天）的日程列表。</summary>
        public ObservableCollection<SysSchedule> RecentSchedules { get; } = new();

        /// <summary>近 3 天日程统计摘要文字。</summary>
        public string ScheduleSummary
        {
            get => _scheduleSummary;
            private set => SetProperty(ref _scheduleSummary, value);
        }

        /// <summary>从 API 拉取日程并重建近 3 天统计（每次激活工作台时调用）。</summary>
        public async Task ReloadAsync()
        {
            try
            {
                await _data.LoadSchedulesAsync();
            }
            catch
            {
                // 加载失败时忽略，工作台展示空列表
            }
            BuildSchedules();
        }

        private void BuildSchedules()
        {
            var today = DateTime.Today;
            var list = _data.Schedules
                .Where(s => s.ScheduleDate >= today && s.ScheduleDate < today.AddDays(3))
                .OrderBy(s => s.ScheduleDate)
                .ThenBy(s => s.StartTime)
                .ToList();

            RecentSchedules.Clear();
            foreach (var s in list) RecentSchedules.Add(s);

            var todayCount = list.Count(s => s.ScheduleDate.Date == today);
            var pendingCount = list.Count(s => s.IsCompleted == 0);
            ScheduleSummary = list.Count == 0
                ? "近 3 天暂无日程安排"
                : $"共 {list.Count} 条（今天 {todayCount} 条，未完成 {pendingCount} 条）";
        }

        private static string TimeGreeting()
        {
            var h = DateTime.Now.Hour;
            if (h < 6) return "夜深了";
            if (h < 12) return "早上好";
            if (h < 14) return "中午好";
            if (h < 18) return "下午好";
            return "晚上好";
        }

        // 图标 Path 几何数据
        public static class Icons
        {
            public const string Users = "M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5s-3 1.34-3 3 1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z";
            public const string Box = "M21 5l-9-3-9 3 9 3 9-3zm-11 4.4L3 6.6v11.1L10 21v-11.6zm9 0v11.1l-7 3.3V21l7-3.3V6.6l-7 3.4z";
            public const string Cart = "M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12.9-1.63h7.45c.75 0 1.41-.41 1.75-1.03l3.58-6.49c.08-.14.12-.31.12-.48 0-.55-.45-1-1-1H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z";
            public const string Money = "M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 14H4v-2h16v2zm0-5H4v-2h16v2zm0-5H4V6h16v2z";
            public const string Dashboard = "M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z";
            public const string User = "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z";
            public const string BoxAlt = "M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V5h14v14zM7 9h10v2H7V9zm0 4h6v2H7v-2z";
            public const string Order = "M8 10h8v2H8v-2zm0 4h8v2H8v-2zm-4-8h16v-2l-16-2v2zm14 14h-4c.01 1.12-.32 2.13-1 3 .68.87 1.01 1.88 1 3h4c.01-3.7 0-3.7-1-6zm-3 0h-4c.01 1.12-.32 2.13-1 3 .68.87 1.01 1.88 1 3h4c.01-3.7 0-3.7-1-6z";
            public const string Settings = "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58c.18-.14.23-.41.12-.61l-1.92-3.32c-.12-.22-.37-.29-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54c-.04-.24-.24-.41-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58c-.18.14-.23.41-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z";
            public const string Logout = "M17 7l-1.41 1.41L18.17 11H8v2h10.17l-2.58 2.58L17 17l5-5zM4 5h8V3H4c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h8v-2H4V5z";
            public const string Bell = "M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z";
            public const string Search = "M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z";
        }
    }
}