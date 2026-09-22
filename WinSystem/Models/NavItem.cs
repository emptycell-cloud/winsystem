using System.Collections.ObjectModel;
using WinSystem.ViewModels;

namespace WinSystem.Models
{
    /// <summary>侧边导航菜单项，支持一级/二级层级。</summary>
    public class NavItem : ViewModelBase
    {
        private bool _isActive;
        private bool _isExpanded;
        private int _unreadCount;

        public string Title { get; set; } = "";
        public string IconData { get; set; } = "";
        public string PageKey { get; set; } = "";

        /// <summary>是否为一级菜单（含子菜单）。</summary>
        public bool IsGroupHeader { get; set; }

        /// <summary>子菜单（二级项）。</summary>
        public ObservableCollection<NavItem> Children { get; } = new();

        /// <summary>是否含有子菜单。</summary>
        public bool HasChildren => Children.Count > 0;

        /// <summary>一级菜单是否展开。</summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>未读数量（气泡），如“我的消息”未读条数。</summary>
        public int UnreadCount
        {
            get => _unreadCount;
            set
            {
                if (SetProperty(ref _unreadCount, value))
                {
                    OnPropertyChanged(nameof(UnreadVisible));
                    OnPropertyChanged(nameof(UnreadText));
                }
            }
        }

        /// <summary>是否显示未读气泡（大于 0 时显示）。</summary>
        public bool UnreadVisible => _unreadCount > 0;

        /// <summary>未读气泡文本（超过 99 显示 99+）。</summary>
        public string UnreadText => _unreadCount > 99 ? "99+" : _unreadCount.ToString();
    }
}