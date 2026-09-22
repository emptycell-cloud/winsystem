namespace WinSystem.Models
{
    /// <summary>我的消息列表项（接收端，来自 /api/messages/mine）。</summary>
    public class MyMessageItem
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public int Type { get; set; }
        public int Status { get; set; }
        public int Scope { get; set; }
        public string Sender { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsRead { get; set; }
        public string TypeText { get; set; } = "";
        public string ScopeText { get; set; } = "";

        /// <summary>发布时间展示文本。</summary>
        public string CreatedText => CreatedAt.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>我的消息分页结果。</summary>
    public class MyMessagePage
    {
        public int Total { get; set; }
        public List<MyMessageItem> Items { get; set; } = new();
    }

    /// <summary>我的未读消息数量。</summary>
    public class MyMessageCount
    {
        public int Count { get; set; }
    }
}
