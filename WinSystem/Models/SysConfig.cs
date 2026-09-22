namespace WinSystem.Models
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

        /// <summary>类型文字：1字符串 2数字 3布尔 4JSON。</summary>
        public string TypeText => ConfigType switch
        {
            2 => "数字",
            3 => "布尔",
            4 => "JSON",
            _ => "字符串"
        };

        /// <summary>来源文字：1系统内置 0自定义。</summary>
        public string SystemText => IsSystem == 1 ? "系统内置" : "自定义";
    }
}
