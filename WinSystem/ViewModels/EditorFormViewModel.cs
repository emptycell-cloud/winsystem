namespace WinSystem.ViewModels
{
    /// <summary>表单编辑器 ViewModel 基类。</summary>
    public abstract class EditorFormViewModel : ViewModelBase
    {
        private string _title = "";
        private string _message = "";

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        /// <summary>窗口“确定”时校验，返回是否合法。</summary>
        public abstract bool Validate();
    }
}