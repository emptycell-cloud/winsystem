using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;
using WinSystem.Services;

namespace WinSystem.ViewModels
{
    /// <summary>发布范围目标选项基类（勾选多选）。</summary>
    public abstract class TargetItemBase : ViewModelBase
    {
        public string Id { get; }
        public string Name { get; }
        public int TargetType { get; }
        private bool _isSelected;

        protected TargetItemBase(string id, string name, int targetType)
        {
            Id = id;
            Name = name;
            TargetType = targetType;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value)) OnSelectionChanged(value);
            }
        }

        /// <summary>勾选状态变化时回调（供树节点级联子节点）。</summary>
        protected virtual void OnSelectionChanged(bool selected) { }
    }

    /// <summary>扁平目标选项（角色）。</summary>
    public class TargetOption : TargetItemBase
    {
        public TargetOption(string id, string name, int targetType) : base(id, name, targetType) { }
    }

    /// <summary>树形目标节点（组织部门树，勾选父节点级联子节点）。</summary>
    public class TargetTreeNode : TargetItemBase
    {
        public TargetTreeNode(string id, string name, int targetType) : base(id, name, targetType) { }

        /// <summary>子节点。</summary>
        public ObservableCollection<TargetTreeNode> Children { get; } = new();

        /// <summary>是否有子节点（树模板展开用）。</summary>
        public bool HasChildren => Children.Count > 0;

        protected override void OnSelectionChanged(bool selected)
        {
            foreach (var child in Children) child.IsSelected = selected;
        }
    }

    /// <summary>用户搜索结果勾选项：勾选即加入已选列表，取消即移除。</summary>
    public class SearchUserOption : ViewModelBase
    {
        private readonly Action<SearchUserOption, bool> _onSelectionChanged;
        private bool _isSelected;

        public SearchUserOption(string id, string name, string username, string orgName,
            Action<SearchUserOption, bool> onSelectionChanged)
        {
            Id = id;
            Name = name;
            Username = username;
            OrgName = orgName ?? "";
            _onSelectionChanged = onSelectionChanged;
        }

        public string Id { get; }
        public string Name { get; }
        public string Username { get; }
        public string OrgName { get; }

        /// <summary>展示文本：姓名（账号 · 部门）。</summary>
        public string Display => string.IsNullOrEmpty(OrgName)
            ? $"{Name}（{Username}）"
            : $"{Name}（{Username} · {OrgName}）";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value)) _onSelectionChanged(this, value);
            }
        }
    }

    /// <summary>消息编辑表单：新增/编辑消息（sys_message），支持选择发布范围目标。</summary>
    public class MessageEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] TypeOptions = { "通知", "公告", "提醒" };
        public static readonly string[] StatusOptions = { "已发布", "草稿" };
        public static readonly string[] ScopeOptions = { "全部用户", "指定角色", "指定部门", "指定用户" };

        private readonly DataService _data;
        private bool _isNew;
        private string _title;
        private string _content;
        private string _remark;
        private int _type;
        private int _status;
        private int _scope;
        private string _searchKeyword = "";
        private ObservableCollection<TargetItemBase> _targetItems = new();

        public MessageEditorViewModel(DataService data, SysMessage? message)
        {
            _data = data;
            _isNew = message == null;
            _title = message?.Title ?? "";
            _content = message?.Content ?? "";
            _remark = message?.Remark ?? "";
            _type = message?.Type ?? 1;
            _status = message?.Status ?? 1;
            _scope = message?.Scope ?? 1;
            Title = _isNew ? "新增消息" : "编辑消息";
            SearchUsersCommand = new RelayCommand(async _ => await SearchUsersAsync());
            RemoveSelectedUserCommand = new RelayCommand(o => RemoveSelectedUser(o as MessageTarget));
            RebuildTargets(message?.Targets ?? new List<MessageTarget>());
        }

        public static MessageEditorViewModel CreateNew(DataService data) => new(data, null);

        public string MessageTitle { get => _title; set => SetProperty(ref _title, value); }
        public string Content { get => _content; set => SetProperty(ref _content, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        public ICommand SearchUsersCommand { get; }
        public ICommand RemoveSelectedUserCommand { get; }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        public string Type
        {
            get => TypeOptions[Math.Clamp(_type - 1, 0, TypeOptions.Length - 1)];
            set { var i = Array.IndexOf(TypeOptions, value); if (i >= 0) _type = i + 1; }
        }

        public string Status
        {
            get => StatusOptions[_status == 1 ? 0 : 1];
            set { var i = Array.IndexOf(StatusOptions, value); if (i >= 0) _status = i == 0 ? 1 : 2; }
        }

        public string Scope
        {
            get => ScopeOptions[Math.Clamp(_scope - 1, 0, ScopeOptions.Length - 1)];
            set
            {
                var i = Array.IndexOf(ScopeOptions, value);
                if (i >= 0 && _scope != i + 1)
                {
                    _scope = i + 1;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasTargets));
                    OnPropertyChanged(nameof(HasFlatTargets));
                    OnPropertyChanged(nameof(HasTreeTargets));
                    OnPropertyChanged(nameof(HasUserPicker));
                    OnPropertyChanged(nameof(TargetHint));
                    SearchKeyword = "";
                    SearchResults.Clear();
                    RebuildTargets(new List<MessageTarget>());
                }
            }
        }

        /// <summary>范围目标选项列表（角色为扁平项，部门为树节点）。</summary>
        public ObservableCollection<TargetItemBase> TargetItems
        {
            get => _targetItems;
            private set => SetProperty(ref _targetItems, value);
        }

        /// <summary>用户搜索结果（指定用户选择器）。</summary>
        public ObservableCollection<SearchUserOption> SearchResults { get; } = new();

        /// <summary>已选接收用户（指定用户选择器）。</summary>
        public ObservableCollection<MessageTarget> SelectedUsers { get; } = new();

        /// <summary>已选用户数。</summary>
        public int SelectedUserCount => SelectedUsers.Count;

        /// <summary>是否显示目标选择区（scope=1 全部用户时不显示）。</summary>
        public bool HasTargets => _scope > 1;

        /// <summary>是否使用扁平勾选列表（角色）。</summary>
        public bool HasFlatTargets => _scope == 2;

        /// <summary>是否使用树控件（部门）。</summary>
        public bool HasTreeTargets => _scope == 3;

        /// <summary>是否使用用户搜索选择器。</summary>
        public bool HasUserPicker => _scope == 4;

        /// <summary>目标选择区提示/摘要文本。</summary>
        public string TargetHint => _scope switch
        {
            2 => "选择接收消息的角色",
            3 => "选择接收消息的部门（勾选父部门将级联其子部门）",
            4 => "按姓名或账号搜索并勾选接收用户",
            _ => "全部用户均可收到该消息"
        };

        /// <summary>当前已选目标数（角色/部门 + 已选用户）。</summary>
        public int SelectedTargetCount
        {
            get
            {
                int count = 0;
                void Walk(IEnumerable<TargetItemBase> items)
                {
                    foreach (var it in items)
                    {
                        if (it.IsSelected) count++;
                        if (it is TargetTreeNode node) Walk(node.Children);
                    }
                }
                Walk(TargetItems);
                return count + SelectedUsers.Count;
            }
        }

        private void RebuildTargets(List<MessageTarget> existing)
        {
            SelectedUsers.Clear();
            var selectedIds = existing.Select(t => (t.TargetType, t.TargetId)).ToHashSet();
            var items = new List<TargetItemBase>();
            switch (_scope)
            {
                case 2: // 角色（扁平）
                    foreach (var r in _data.Roles)
                        items.Add(new TargetOption(r.Id, r.Name, 2) { IsSelected = selectedIds.Contains((2, r.Id)) });
                    break;
                case 3: // 部门（树 + 复选框）
                    foreach (var root in _data.Organizations)
                        items.Add(BuildOrgNode(root, selectedIds));
                    break;
                case 4: // 用户（搜索选择器，预选已存目标）
                    foreach (var t in existing.Where(t => t.TargetType == 4))
                        SelectedUsers.Add(new MessageTarget { TargetType = 4, TargetId = t.TargetId, TargetName = t.TargetName ?? t.TargetId });
                    break;
            }
            TargetItems = new ObservableCollection<TargetItemBase>(items);
        }

        /// <summary>把组织树节点递归转成带复选框的树形目标节点。</summary>
        private static TargetTreeNode BuildOrgNode(Models.Organization org, HashSet<(int, string)> selectedIds)
        {
            var node = new TargetTreeNode(org.Id, org.Name, 3)
            {
                IsSelected = selectedIds.Contains((3, org.Id))
            };
            foreach (var child in org.Children)
                node.Children.Add(BuildOrgNode(child, selectedIds));
            return node;
        }

        /// <summary>按关键字搜索用户并填充结果列表（已选用户保持勾选状态）。</summary>
        private async Task SearchUsersAsync()
        {
            var keyword = SearchKeyword?.Trim() ?? "";
            if (keyword.Length == 0)
            {
                SearchResults.Clear();
                return;
            }
            var list = await _data.SearchUsersAsync(keyword);
            var selected = SelectedUsers.Select(u => u.TargetId).ToHashSet();
            SearchResults.Clear();
            foreach (var item in list)
            {
                SearchResults.Add(new SearchUserOption(item.Id, item.Name, item.Username, item.OrganizationName ?? "",
                    OnUserSelectionChanged)
                {
                    IsSelected = selected.Contains(item.Id)
                });
            }
        }

        /// <summary>搜索结果勾选状态变化：加入/移出已选用户列表。</summary>
        private void OnUserSelectionChanged(SearchUserOption opt, bool selected)
        {
            if (selected)
            {
                if (!SelectedUsers.Any(u => u.TargetId == opt.Id))
                    SelectedUsers.Add(new MessageTarget { TargetType = 4, TargetId = opt.Id, TargetName = opt.Name });
            }
            else
            {
                var existing = SelectedUsers.FirstOrDefault(u => u.TargetId == opt.Id);
                if (existing != null) SelectedUsers.Remove(existing);
            }
            OnPropertyChanged(nameof(SelectedUserCount));
        }

        /// <summary>从已选用户移除（同时取消搜索结果对应勾选）。</summary>
        private void RemoveSelectedUser(MessageTarget? target)
        {
            if (target == null) return;
            SelectedUsers.Remove(target);
            var opt = SearchResults.FirstOrDefault(o => o.Id == target.TargetId);
            if (opt != null && opt.IsSelected) opt.IsSelected = false;
            OnPropertyChanged(nameof(SelectedUserCount));
        }

        /// <summary>收集所有选中目标（角色/部门 + 已选用户）。</summary>
        private List<MessageTarget> CollectSelected()
        {
            var result = new List<MessageTarget>();
            void Walk(IEnumerable<TargetItemBase> items)
            {
                foreach (var it in items)
                {
                    if (it.IsSelected)
                        result.Add(new MessageTarget { TargetType = it.TargetType, TargetId = it.Id, TargetName = it.Name });
                    if (it is TargetTreeNode node) Walk(node.Children);
                }
            }
            Walk(TargetItems);
            foreach (var u in SelectedUsers)
                result.Add(new MessageTarget { TargetType = 4, TargetId = u.TargetId, TargetName = u.TargetName });
            return result;
        }

        public bool IsNew => _isNew;

        public SysMessage BuildEntity() => new()
        {
            Title = MessageTitle.Trim(),
            Content = Content.Trim(),
            Type = _type,
            Status = _status,
            Scope = _scope,
            Remark = Remark.Trim(),
            Targets = CollectSelected(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(SysMessage message)
        {
            message.Title = MessageTitle.Trim();
            message.Content = Content.Trim();
            message.Type = _type;
            message.Status = _status;
            message.Scope = _scope;
            message.Remark = Remark.Trim();
            message.Targets = CollectSelected();
            message.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(MessageTitle)) { Message = "请填写消息标题。"; return false; }
            if (string.IsNullOrWhiteSpace(Content)) { Message = "请填写消息内容。"; return false; }
            if (_scope > 1 && SelectedTargetCount == 0)
            {
                Message = _scope switch
                {
                    2 => "请至少选择一个接收角色。",
                    3 => "请至少选择一个接收部门。",
                    4 => "请至少选择一个接收用户。",
                    _ => "请选择发布范围目标。"
                };
                return false;
            }
            Message = "";
            return true;
        }
    }
}
