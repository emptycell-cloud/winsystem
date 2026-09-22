namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_role 系统角色表。</summary>
    public class Role
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int DataScope { get; set; } = 2;
        public int IsBuiltIn { get; set; }
        public int Status { get; set; } = 1;
        public int SortOrder { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>对应数据库 sys_role_menu 角色功能权限表。</summary>
    public class RoleMenu
    {
        public long Id { get; set; }
        public string RoleId { get; set; } = "";
        public string MenuKey { get; set; } = "";
        public int PermissionState { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>对应数据库 sys_account_role 账号角色关联表。</summary>
    public class AccountRole
    {
        public long Id { get; set; }
        public string AccountId { get; set; } = "";
        public string RoleId { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}