using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WinSystem.Converters
{
    /// <summary>将业务状态字符串映射为语义色文字画刷。</summary>
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value?.ToString() ?? "";
            Brush brush = (Brush)Application.Current.FindResource("TextSecondaryBrush");
            if (status.Contains("已") || status.Contains("完成") || status.Contains("正常") || status == "启用" || status == "有效" || status == "成功")
                brush = (Brush)Application.Current.FindResource("SuccessBrush");
            else if (status.Contains("待") || status.Contains("处理中") || status == "启用中" || status == "禁用中")
                brush = (Brush)Application.Current.FindResource("WarningBrush");
            else if (status.Contains("退") || status.Contains("关闭") || status == "禁用" || status == "作废" || status.Contains("失败"))
                brush = (Brush)Application.Current.FindResource("DangerBrush");
            else if (status.Contains("锁定") || status.Contains("待审核") || status.Contains("待发货"))
                brush = (Brush)Application.Current.FindResource("WarningBrush");
            else if (status.Contains("待激活"))
                brush = (Brush)Application.Current.FindResource("InfoBrush");
            return brush;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>将业务状态字符串映射为语义色浅色底 + 对应前景（用于状态标签）。</summary>
    public class StatusToTagConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value?.ToString() ?? "";
            Brush background, foreground;

            if (status.Contains("已") || status.Contains("完成") || status.Contains("正常") || status == "启用" || status == "有效" || status == "成功")
            {
                background = (Brush)Application.Current.FindResource("PanelGreenBrush");
                foreground = (Brush)Application.Current.FindResource("SuccessBrush");
            }
            else if (status.Contains("待激活"))
            {
                background = (Brush)Application.Current.FindResource("PanelBlueBrush");
                foreground = (Brush)Application.Current.FindResource("InfoBrush");
            }
            else if (status.Contains("锁定") || status.Contains("待") || status.Contains("中") || status == "启用中")
            {
                background = (Brush)Application.Current.FindResource("PanelOrangeBrush");
                foreground = (Brush)Application.Current.FindResource("WarningBrush");
            }
            else if (status.Contains("退") || status.Contains("关闭") || status == "禁用" || status == "作废" || status.Contains("失败"))
            {
                background = (Brush)Application.Current.FindResource("PanelRedBrush");
                foreground = (Brush)Application.Current.FindResource("DangerBrush");
            }
            else
            {
                background = (Brush)Application.Current.FindResource("PanelBlueBrush");
                foreground = (Brush)Application.Current.FindResource("PrimaryDarkBrush");
            }
            return paramIsBrush(parameter) ? background : foreground;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();

        private static bool paramIsBrush(object? parameter) => string.Equals(parameter?.ToString(), "background",
                                                                           StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>布尔反转后转可见性。</summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var inverted = value is true ? false : true;
            return inverted ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is Visibility.Visible ? false : true;
    }

    /// <summary>布尔转可见性：true 显示，false 折叠。</summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is Visibility.Visible;
    }

    /// <summary>空文本/集合 -> Collapsed。</summary>
    public class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Collapsed;
            if (value is string s) return string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
            if (value is System.Collections.IEnumerable e) return e.GetEnumerator().MoveNext() ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>金额格式化：¥1,234.56</summary>
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null) return "¥0.00";
            var number = decimal.TryParse(value.ToString(), out var d) ? d : 0m;
            return "¥" + number.ToString("N2");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>百分比(0-100) → 像素宽度（用于横向进度条）。</summary>
    public class PercentToWidthConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var percent = double.TryParse(value?.ToString(), out var p) ? p : 0;
            var max = parameter is string s && double.TryParse(s, out var m) && m > 0 ? m : 200.0;
            var width = Math.Max(0, Math.Min(percent, 100) * max / 100.0);
            return width;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>资源键字符串 → 对应画刷。</summary>
    public class NamedBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var key = value?.ToString();
            if (string.IsNullOrEmpty(key)) return Binding.DoNothing;
            var brush = Application.Current.TryFindResource(key);
            return brush ?? Binding.DoNothing;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>将十六进制颜色字符串（如 #3B82F6）转换为画刷，供日程颜色指示点使用。</summary>
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                try
                {
                    var c = (Color)ColorConverter.ConvertFromString(hex);
                    return new SolidColorBrush(c);
                }
                catch { /* 忽略无效颜色值 */ }
            }
            return (Brush)Application.Current.FindResource("PrimaryBrush");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>取姓名/字符串首字符（用于头像）。</summary>
    public class InitialConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var s = value?.ToString();
            return string.IsNullOrEmpty(s) ? "?" : s.Trim().Substring(0, 1).ToUpperInvariant();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}