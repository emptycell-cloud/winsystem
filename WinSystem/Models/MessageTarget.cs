namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_message_target 消息发布范围目标表。</summary>
    public class MessageTarget
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public int TargetType { get; set; }  // 2=角色 3=部门 4=用户
        public string TargetId { get; set; } = "";
        public string? TargetName { get; set; }

        /// <summary>目标类型文字：2角色 3部门 4用户。</summary>
        public string TargetTypeText => TargetType switch
        {
            2 => "角色",
            3 => "部门",
            4 => "用户",
            _ => ""
        };
    }
}
