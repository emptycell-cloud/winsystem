namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_message 消息通知表。</summary>
    public class SysMessage
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int Type { get; set; } = 1;   // 1=通知 2=公告 3=提醒
        public int Status { get; set; } = 1;  // 1=已发布 2=草稿
        public int Scope { get; set; } = 1;   // 1=全部用户 2=指定角色 3=指定部门 4=指定用户
        public string? Sender { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<MessageTarget> Targets { get; set; } = new();

        /// <summary>消息类型文字：1通知 2公告 3提醒。</summary>
        public string TypeText => Type switch
        {
            1 => "通知",
            2 => "公告",
            3 => "提醒",
            _ => "通知"
        };

        /// <summary>状态文字：1已发布 2草稿。</summary>
        public string StatusText => Status == 1 ? "已发布" : "草稿";

        /// <summary>发布范围文字：1全部用户 2指定角色 3指定部门 4指定用户。</summary>
        public string ScopeText => Scope switch
        {
            1 => "全部用户",
            2 => "指定角色",
            3 => "指定部门",
            4 => "指定用户",
            _ => "全部用户"
        };

        /// <summary>发布时间文本。</summary>
        public string CreatedText => CreatedAt.ToString("yyyy-MM-dd HH:mm");

        /// <summary>内容摘要（列表展示用）。</summary>
        public string ContentPreview => string.IsNullOrEmpty(Content) ? ""
            : (Content.Length > 60 ? Content[..60] + "…" : Content);
    }
}
