namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_message_target 消息发布范围目标表。</summary>
    public class MessageTarget
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public int TargetType { get; set; }  // 2=角色 3=部门 4=用户
        public string TargetId { get; set; } = "";
    }
}
