using System.Text.Json.Serialization;

namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_message 消息通知表。</summary>
    public class SysMessage
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int Type { get; set; } = 1;      // 1=通知 2=公告 3=提醒
        public int Status { get; set; } = 1;     // 1=已发布 2=草稿
        public int Scope { get; set; } = 1;      // 1=全部用户 2=指定角色 3=指定部门 4=指定用户
        public string? Sender { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public string TypeText => Type switch
        {
            1 => "通知",
            2 => "公告",
            3 => "提醒",
            _ => "通知"
        };

        [JsonIgnore]
        public string StatusText => Status == 1 ? "已发布" : "草稿";

        [JsonIgnore]
        public string ScopeText => Scope switch
        {
            1 => "全部用户",
            2 => "指定角色",
            3 => "指定部门",
            4 => "指定用户",
            _ => "全部用户"
        };
    }
}
