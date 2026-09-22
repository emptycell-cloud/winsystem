using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_organization 组织机构表（树形结构）。</summary>
    public class Organization
    {
        public string Id { get; set; } = "";
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? ParentId { get; set; }
        public int Type { get; set; } = 1;
        public string? Manager { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; } = 1;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>子组织（仅 API 返回树时填充，不映射数据库）。</summary>
        [NotMapped]
        public List<Organization> Children { get; set; } = new();

        /// <summary>上级组织名称（仅配置列表查询时填充，不映射数据库）。</summary>
        [NotMapped]
        public string? ParentName { get; set; }
    }
}