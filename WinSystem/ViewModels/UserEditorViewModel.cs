using System.Collections.ObjectModel;
using WinSystem.Models;
using WinSystem.Services;

namespace WinSystem.ViewModels
{
    public class UserEditorViewModel : EditorFormViewModel
    {
        private string _name;
        private string _username;
        private string _password;
        private string _position;
        private string _email;
        private string _phone;
        private string _organizationId;
        private string _remark;
        private int _status;
        private Organization? _selectedOrg;

        public static readonly string[] StatusOptions = { "正常", "禁用", "待激活" };
        private readonly List<string>? _takenUsernames;
        private readonly bool _isNew;
        private string _selectedRoleId = "";
        private List<Role> _roles;
        private Role? _selectedRole;

        public UserEditorViewModel(User? user, List<string>? takenUsernames = null,
            List<Role>? roles = null, string? roleId = null)
        {
            _isNew = user == null;
            _takenUsernames = takenUsernames;
            _roles = roles ?? new List<Role>();
            _selectedRoleId = roleId ?? "";
            _selectedRole = string.IsNullOrEmpty(_selectedRoleId) ? null : _roles.FirstOrDefault(r => r.Id == _selectedRoleId);
            _name = user?.Name ?? "";
            _username = user?.Username ?? "";
            _password = "";
            _position = user?.Position ?? "";
            _email = user?.Email ?? "";
            _phone = user?.Phone ?? "";
            _organizationId = user?.OrganizationId ?? "";
            _remark = user?.Remark ?? "";
            _status = user?.Status ?? 1;
            Title = _isNew ? "新增用户" : "编辑用户";

            // 加载组织树
            foreach (var root in Session.Data.Organizations) Organizations.Add(root);
            _selectedOrg = FindOrg(_organizationId);
        }

        public static UserEditorViewModel CreateNew(List<string> takenUsernames, List<Role>? roles = null)
            => new(null, takenUsernames, roles);

        public ObservableCollection<Organization> Organizations { get; } = new();

        /// <summary>角色下拉项。</summary>
        public List<Role> Roles => _roles;

        /// <summary>当前选中的角色（对象引用，反选可靠）。</summary>
        public Role? SelectedRole
        {
            get => _selectedRole;
            set { if (SetProperty(ref _selectedRole, value)) _selectedRoleId = value?.Id ?? ""; }
        }

        /// <summary>组织树选中项。</summary>
        public Organization? SelectedOrg
        {
            get => _selectedOrg;
            set
            {
                if (SetProperty(ref _selectedOrg, value))
                    OrganizationId = value?.Id ?? "";
            }
        }

        private Organization? FindOrg(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var root in Organizations)
            {
                var found = FindRecursive(root, id);
                if (found != null) return found;
            }
            return null;
        }

        private static Organization? FindRecursive(Organization node, string id)
        {
            if (node.Id == id) return node;
            foreach (var child in node.Children)
            {
                var found = FindRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Position { get => _position; set => SetProperty(ref _position, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string OrganizationId { get => _organizationId; set => SetProperty(ref _organizationId, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        /// <summary>状态选中项：1正常 2禁用 3待激活。</summary>
        public string Status
        {
            get => StatusOptions[Math.Clamp(_status - 1, 0, StatusOptions.Length - 1)];
            set
            {
                var idx = Array.IndexOf(StatusOptions, value);
                if (idx >= 0 && SetProperty(ref _status, idx + 1)) { }
            }
        }

        public int StatusValue => _status;
        public bool IsNew => _isNew;

        public User BuildEntity() => new()
        {
            Name = Name.Trim(),
            Username = Username.Trim(),
            PasswordHash = "123456",
            Position = Position.Trim(),
            Email = Email.Trim(),
            Phone = Phone.Trim(),
            OrganizationId = OrganizationId.Trim(),
            Remark = Remark.Trim(),
            Status = _status,
            RoleIds = string.IsNullOrEmpty(_selectedRoleId) ? null : new List<string> { _selectedRoleId },
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(User user)
        {
            user.Name = Name.Trim();
            user.Username = Username.Trim();
            user.Position = Position.Trim();
            user.Email = Email.Trim();
            user.Phone = Phone.Trim();
            user.OrganizationId = OrganizationId.Trim();
            user.Remark = Remark.Trim();
            user.Status = _status;
            user.RoleIds = string.IsNullOrEmpty(_selectedRoleId) ? null : new List<string> { _selectedRoleId };
            user.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写姓名。"; return false; }
            if (string.IsNullOrWhiteSpace(Username)) { Message = "请填写登录名。"; return false; }
            if (_isNew && _takenUsernames != null &&
                _takenUsernames.Any(u => string.Equals(u, Username.Trim(), StringComparison.OrdinalIgnoreCase)))
            { Message = "该登录名已存在。"; return false; }
            if ((SelectedRole == null || string.IsNullOrEmpty(SelectedRole.Id)))
            { Message = "请选择角色。"; return false; }
            if (string.IsNullOrWhiteSpace(_organizationId))
            { Message = "请选择所属组织。"; return false; }
            Message = "";
            return true;
        }
    }
}