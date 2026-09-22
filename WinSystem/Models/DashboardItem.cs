namespace WinSystem.Models
{
    /// <summary>仪表盘统计卡片。</summary>
    public class StatCard
    {
        public string Title { get; set; } = "";
        public string Value { get; set; } = "";
        public string IconData { get; set; } = "";
        public string IconBrush { get; set; } = "PrimaryBrush";
        public string PanelBrush { get; set; } = "PanelBlueBrush";
        public string DeltaText { get; set; } = "";
        public string DeltaBrush { get; set; } = "TextMutedBrush";
        public string DeltaIcon { get; set; } = "↑";
    }

    /// <summary>月度营收柱条目。</summary>
    public class MonthlyBar
    {
        public string Label { get; set; } = "";
        public double Height { get; set; }
        public string Display { get; set; } = "";
        public string Brush { get; set; } = "PrimaryBrush";
    }

    /// <summary>品类占比条目。</summary>
    public class CategoryBar
    {
        public string Name { get; set; } = "";
        public double Percent { get; set; }
        public int Count { get; set; }
        public string Brush { get; set; } = "PrimaryBrush";
    }
}