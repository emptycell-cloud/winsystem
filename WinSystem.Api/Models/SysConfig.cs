namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_config 系统参数表。</summary>
    public class SysConfig
    {
        public long Id { get; set; }
        public string ConfigKey { get; set; } = "";
        public string ConfigValue { get; set; } = "";
        public string ConfigName { get; set; } = "";
        public int ConfigType { get; set; } = 1;
        public int IsSystem { get; set; } = 0;
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
