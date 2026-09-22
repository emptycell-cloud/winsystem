namespace WinSystem.Api.Models
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
    }
}
