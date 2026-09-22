using System.Text.Encodings.Web;
using System.Text.Json;

namespace WinSystem.Models
{
    /// <summary>对应数据库 sys_operation_log 操作日志表。</summary>
    public class SysOperationLog
    {
        public long Id { get; set; }
        public string? AccountId { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string Module { get; set; } = "";
        public string Action { get; set; } = "";
        public string? Method { get; set; }
        public string? Path { get; set; }
        public string? Detail { get; set; }
        public int Status { get; set; } = 1;
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public long DurationMs { get; set; }
        public string? BeforeData { get; set; }
        public string? AfterData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>列表行号（跨页连续编号，由列表 ViewModel 填充）。</summary>
        public int Index { get; set; }

        /// <summary>操作人文字：姓名（账号）。</summary>
        public string OperatorText =>
            string.IsNullOrWhiteSpace(Name) ? (Username ?? "-")
            : string.IsNullOrWhiteSpace(Username) ? Name
            : $"{Name}（{Username}）";

        /// <summary>执行状态文字。</summary>
        public string StatusText => Status == 2 ? "失败" : "成功";

        /// <summary>操作时间文字。</summary>
        public string CreatedText => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>耗时文字，如 12 ms / 1.5 秒。</summary>
        public string DurationText =>
            DurationMs >= 1000 ? $"{DurationMs / 1000.0:0.##} 秒" : $"{DurationMs} ms";

        /// <summary>详情文字（空则显示“-”）。</summary>
        public string DetailText => string.IsNullOrWhiteSpace(Detail) ? "-" : Detail;

        /// <summary>操作前数据快照（格式化换行展示，空则显示“（无）”）。</summary>
        public string BeforeDataText => FormatJson(BeforeData);

        /// <summary>操作后数据快照（格式化换行展示，空则显示“（无）”）。</summary>
        public string AfterDataText => FormatJson(AfterData);

        /// <summary>将快照 JSON 格式化为多行缩进文本，保证详情页自动换行且中文不被转义；非 JSON 原样返回。</summary>
        private static string FormatJson(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "（无）";
            try
            {
                using var doc = JsonDocument.Parse(text);
                return JsonSerializer.Serialize(doc.RootElement, PrettyJsonOptions);
            }
            catch
            {
                return text;
            }
        }

        private static readonly JsonSerializerOptions PrettyJsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>IP 文字。</summary>
        public string IpText => string.IsNullOrWhiteSpace(Ip) ? "-" : Ip;

        /// <summary>UserAgent 文字。</summary>
        public string UserAgentText => string.IsNullOrWhiteSpace(UserAgent) ? "-" : UserAgent;
    }
}
