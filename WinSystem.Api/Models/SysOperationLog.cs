namespace WinSystem.Api.Models
{
    /// <summary>操作日志实体，对应 sys_operation_log 表。</summary>
    public class SysOperationLog
    {
        public long Id { get; set; }
        public string? AccountId { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string Module { get; set; } = "";
        public string Action { get; set; } = "";
        public string? Method { get; set; }
        public string? Path { get; set; }
        public string? Detail { get; set; }
        public int Status { get; set; } = 1;
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public long DurationMs { get; set; }
        public string? BeforeData { get; set; }
        public string? AfterData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
