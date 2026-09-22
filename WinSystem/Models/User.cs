namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_account 系统账号表。</summary>
    public class User
    {
        public string Id { get; set; } = "";
        public string Username { get; set; } = "";
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
        public DateTime? PasswordChangedAt { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>列表行号（跨页连续编号，由列表 ViewModel 填充）。</summary>
        public int Index { get; set; }

        /// <summary>角色 ID 列表（保存到 sys_account_role，客户端提交用）。</summary>
        public List<string>? RoleIds { get; set; }

        /// <summary>状态文字：1正常 2禁用 3待激活。</summary>
        public string StatusText => Status switch
        {
            1 => "正常",
            2 => "禁用",
            3 => "待激活",
            _ => "正常"
        };
    }
}