namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_schedule 日程信息表。</summary>
    public class SysSchedule
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Color { get; set; } = "#3B82F6";
        public string? Location { get; set; }
        public int IsCompleted { get; set; } = 0;
        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public int Status { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>日程日期文字。</summary>
        public string DateText => ScheduleDate.ToString("yyyy-MM-dd");

        /// <summary>时间范围文字，如 09:00-10:00；无开始时间时为“全天”。</summary>
        public string TimeText =>
            string.IsNullOrWhiteSpace(StartTime) ? "全天"
            : string.IsNullOrWhiteSpace(EndTime) ? StartTime
            : $"{StartTime}-{EndTime}";

        /// <summary>完成状态文字。</summary>
        public string StatusText => IsCompleted == 1 ? "已完成" : "未完成";
    }
}
