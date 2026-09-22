using System.Windows.Controls;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    /// <summary>日程管理页：日程列表。</summary>
    public partial class ScheduleView : UserControl
    {
        public ScheduleView()
        {
            InitializeComponent();
            Loaded += async (_, _) =>
            {
                if (DataContext is ScheduleViewModel vm)
                    await vm.ReloadAsync();
            };
        }
    }
}
