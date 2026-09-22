using System.Globalization;

namespace WinSystem.Helpers
{
    /// <summary>公历转农历（中国农历）的静态工具类，用于月历中显示农历日期。</summary>
    public static class ChineseLunar
    {
        private static readonly ChineseLunisolarCalendar Cal = new();

        private static readonly string[] MonthNames =
        {
            "", "正月", "二月", "三月", "四月", "五月", "六月",
            "七月", "八月", "九月", "十月", "冬月", "腊月"
        };

        private static readonly string[] DayNames =
        {
            "", "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
            "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
            "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
        };

        /// <summary>获取指定公历日期的农历日期文本，如“正月十五”“闰四月初一”。</summary>
        public static string GetLunarText(DateTime date)
        {
            int year = Cal.GetYear(date);
            int month = Cal.GetMonth(date);
            int day = Cal.GetDayOfMonth(date);
            var monthName = (Cal.IsLeapMonth(year, month) ? "闰" : "") + MonthNames[month];
            var dayName = day <= 30 ? DayNames[day] : day.ToString();
            return monthName + dayName;
        }
    }
}
