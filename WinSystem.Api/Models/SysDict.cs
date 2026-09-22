using System.ComponentModel.DataAnnotations.Schema;

namespace WinSystem.Api.Models
{
    /// <summary>对应数据库 sys_dict_type 字典类型表。</summary>
    public class SysDictType
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int Status { get; set; } = 1;
        public int SortOrder { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>该类型下的字典数据数量（查询时填充），数据库无此列。</summary>
        [NotMapped]
        public int DataCount { get; set; }
    }

    /// <summary>对应数据库 sys_dict_data 字典数据表。</summary>
    public class SysDictData
    {
        public long Id { get; set; }
        public string DictCode { get; set; } = "";
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public string? Color { get; set; }
        public int Status { get; set; } = 1;
        public int SortOrder { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
