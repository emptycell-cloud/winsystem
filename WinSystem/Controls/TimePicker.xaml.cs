using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WinSystem.Controls
{
    /// <summary>时间选择控件：小时/分钟下拉 + 「全天」开关。Time 为 "HH:mm" 字符串，空字符串表示全天。
    /// 内部完全由代码驱动，不依赖绑定，避免初始化时序导致已选值无法回显。</summary>
    public partial class TimePicker : UserControl
    {
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(string), typeof(TimePicker),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTimeChanged));

        private bool _updating;

        public TimePicker()
        {
            InitializeComponent();
            HourCombo.ItemsSource = Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToList();
            MinuteCombo.ItemsSource = Enumerable.Range(0, 60).Select(m => m.ToString("00")).ToList();
        }

        /// <summary>选中的时间，格式 "HH:mm"，空字符串表示全天。</summary>
        public string Time
        {
            get => (string)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value ?? "");
        }

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((TimePicker)d).SyncFromTime(e.NewValue as string);

        /// <summary>加载完成后再次同步，确保下拉框已填充条目时选中值能正确回显。</summary>
        private void OnLoaded(object sender, RoutedEventArgs e) => SyncFromTime(Time);

        /// <summary>根据 Time 值同步内部控件状态。</summary>
        private void SyncFromTime(string? time)
        {
            _updating = true;
            if (string.IsNullOrWhiteSpace(time))
            {
                AllDayCheck.IsChecked = true;
                HourCombo.SelectedItem = null;
                MinuteCombo.SelectedItem = null;
            }
            else
            {
                AllDayCheck.IsChecked = false;
                var parts = time.Split(':');
                var h = parts.Length > 0 && int.TryParse(parts[0], out var hh) ? hh : 9;
                var m = parts.Length > 1 && int.TryParse(parts[1], out var mm) ? mm : 0;
                HourCombo.SelectedItem = h.ToString("00");
                MinuteCombo.SelectedItem = m.ToString("00");
            }
            _updating = false;
        }

        private void OnHourChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_updating) return;
            AllDayCheck.IsChecked = false;
            PushTime();
        }

        private void OnMinuteChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_updating) return;
            AllDayCheck.IsChecked = false;
            PushTime();
        }

        private void OnAllDayToggled(object sender, RoutedEventArgs e)
        {
            if (_updating) return;
            PushTime();
        }

        /// <summary>根据当前控件状态回写 Time，供双向绑定推送至 ViewModel。</summary>
        private void PushTime()
        {
            if (AllDayCheck.IsChecked == true)
            {
                Time = "";
                return;
            }
            var h = HourCombo.SelectedItem as string ?? "09";
            var m = MinuteCombo.SelectedItem as string ?? "00";
            Time = $"{h}:{m}";
        }
    }
}
