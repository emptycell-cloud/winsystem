using System.Text.Json.Serialization;

namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_account 系统账号表。</summary>
    public class User
    {
        public string Id { get; set; } = "";
        public string Username { get; set; } = "";
        /// <summary>密码哈希：不出现在 JSON 响应与操作日志快照中。</summary>
        [JsonIgnore]
        public string PasswordHash { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public string? OrganizationId { get; set; }
        public int LoginType { get; set; } = 1;
        public int Status { get; set; } = 1;
        public int IsProtected { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public int LoginCount { get; set; }
        /// <summary>Token 版本号：密码修改/重置或账号禁用时递增，旧 JWT 立即失效。</summary>
        public int TokenVersion { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>状态文字：1正常 2锁定 3禁用 4待激活。</summary>
        [JsonIgnore]
        public string StatusText => Status switch
        {
            1 => "正常",
            2 => "锁定",
            3 => "禁用",
            4 => "待激活",
            _ => "正常"
        };
    }
}