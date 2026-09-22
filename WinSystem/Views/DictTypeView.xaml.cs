using System.Windows.Controls;
using WinSystem.ViewModels;

namespace WinSystem.Views
{
    /// <summary>数据字典页：左侧字典类型列表 + 右侧字典数据。</summary>
    public partial class DictTypeView : UserControl
    {
        public DictTypeView()
        {
            InitializeComponent();
            Loaded += async (_, _) =>
            {
                if (DataContext is DictTypeViewModel vm)
                    await vm.ReloadAsync();
            };
        }
    }
}
