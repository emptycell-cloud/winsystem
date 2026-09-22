namespace WinSystem.Models
{
    /// <summary>用户搜索结果项（消息通知选择接收用户用，来自 /api/users/search）。</summary>
    public class UserSearchItem
    {
        public string Id { get; set; } = "";
        public string Username { get; set; } = "";
        public string Name { get; set; } = "";
        public string? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
    }
}
