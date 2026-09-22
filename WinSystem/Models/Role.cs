using System.Collections.ObjectModel;

namespace WinSystem.Models
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

        /// <summary>数据范围文字：1全部 2本部门及以下 3本部门 4仅本人 5自定义。</summary>
        public string DataScopeText => DataScope switch
        {
            1 => "全部",
            2 => "本部门及以下",
            3 => "本部门",
            4 => "仅本人",
            5 => "自定义",
            _ => "未知"
        };

        /// <summary>状态文字：1启用 0停用。</summary>
        public string StatusText => Status == 1 ? "启用" : "停用";

        /// <summary>内置标记文字。</summary>
        public string BuiltInText => IsBuiltIn == 1 ? "内置" : "自定义";
    }
}