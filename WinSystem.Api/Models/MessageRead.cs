namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_message_read 消息已读记录表。</summary>
    public class MessageRead
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public string AccountId { get; set; } = "";
        public DateTime ReadAt { get; set; } = DateTime.Now;
    }
}
