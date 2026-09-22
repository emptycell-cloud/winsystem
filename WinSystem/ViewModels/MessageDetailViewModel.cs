using System.Collections.ObjectModel;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>消息详情 ViewModel：只读展示标题/类型/范围/发送人/时间/内容及发布目标。</summary>
    public class MessageDetailViewModel : ViewModelBase
    {
        private readonly SysMessage _message;

        public MessageDetailViewModel(SysMessage message)
        {
            _message = message;
            Targets = new ObservableCollection<MessageTarget>(message.Targets ?? new List<MessageTarget>());
        }

        public string Title => $"「{_message.Title}」";
        public string TypeText => _message.TypeText;
        public string StatusText => _message.StatusText;
        public string ScopeText => _message.ScopeText;
        public string SenderText => string.IsNullOrEmpty(_message.Sender) ? "—" : _message.Sender;
        public string TimeText => _message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        public string Content => _message.Content;
        public string RemarkText => string.IsNullOrEmpty(_message.Remark) ? "—" : _message.Remark;

        /// <summary>是否有备注（控制备注区显示）。</summary>
        public bool HasRemark => !string.IsNullOrEmpty(_message.Remark);

        /// <summary>发布范围目标列表（scope=1 全部用户时为空）。</summary>
        public ObservableCollection<MessageTarget> Targets { get; }

        /// <summary>是否显示目标列表区。</summary>
        public bool HasTargets => Targets.Count > 0;
    }
}
