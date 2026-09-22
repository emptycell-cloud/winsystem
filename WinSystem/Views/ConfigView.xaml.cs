using System.Windows.Controls;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    /// <summary>系统参数页：系统参数列表。</summary>
    public partial class ConfigView : UserControl
    {
        public ConfigView()
        {
            InitializeComponent();
            Loaded += async (_, _) =>
            {
                if (DataContext is ConfigViewModel vm)
                    await vm.ReloadAsync();
            };
        }
    }
}
