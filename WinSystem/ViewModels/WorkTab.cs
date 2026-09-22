using System.Windows.Input;
using WinSystem.Commands;

namespace WinSystem.ViewModels
{
    /// <summary>主界面工作区选项卡。</summary>
    public class WorkTab : ViewModelBase
    {
        private bool _isActive;
        private readonly Action<WorkTab>? _closeAction;

        public WorkTab(string pageKey, string title, object content, bool canClose, Action<WorkTab>? closeAction = null)
        {
            PageKey = pageKey;
            Title = title;
            Content = content;
            CanClose = canClose;
            _closeAction = closeAction;
            CloseCommand = new RelayCommand(() => _closeAction?.Invoke(this));
        }

        public string PageKey { get; }
        public string Title { get; }
        public object Content { get; }

        /// <summary>是否可关闭（数据中心常驻不可关闭）。</summary>
        public bool CanClose { get; }

        public ICommand CloseCommand { get; }

        /// <summary>是否为当前激活选项卡。</summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}