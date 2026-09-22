using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>组织机构编辑表单：新增/编辑组织记录（sys_organization）。</summary>
    public class OrganizationEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] TypeOptions = { "总部", "中心", "部门" };
        public static readonly string[] StatusOptions = { "启用", "禁用" };

        /// <summary>上级组织下拉选项（null Id 表示根级）。</summary>
        public class ParentOption
        {
            public string Display { get; set; } = "根级（集团总部）";
            public string? Id { get; set; }
        }

        private readonly bool _isNew;
        private readonly List<Organization> _all;
        private string _name;
        private string _code;
        private string _manager;
        private string _phone;
        private string _email;
        private string _description;
        private string _sortOrderText;
        private string? _parentId;
        private int _type;
        private int _status;
        private ParentOption? _selectedParent;

        public OrganizationEditorViewModel(Organization? org, List<Organization> all)
        {
            _isNew = org == null;
            _all = all;
            _name = org?.Name ?? "";
            _code = org?.Code ?? "";
            _manager = org?.Manager ?? "";
            _phone = org?.Phone ?? "";
            _email = org?.Email ?? "";
            _description = org?.Description ?? "";
            _sortOrderText = (org?.SortOrder ?? 0).ToString();
            _parentId = org?.ParentId;
            _type = org?.Type ?? 2;
            _status = org?.Status ?? 1;
            Title = _isNew ? "新增组织" : "编辑组织";

            BuildParentOptions(org?.Id);
            SelectedParent = ParentOptions.FirstOrDefault(o => o.Id == _parentId) ?? ParentOptions[0];
        }

        public static OrganizationEditorViewModel CreateNew(List<Organization> all) => new(null, all);

        public List<ParentOption> ParentOptions { get; } = new();

        public ParentOption? SelectedParent
        {
            get => _selectedParent;
            set { if (SetProperty(ref _selectedParent, value)) _parentId = value?.Id; }
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Code { get => _code; set => SetProperty(ref _code, value); }
        public string Manager { get => _manager; set => SetProperty(ref _manager, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        /// <summary>排序号（文本框字符串，便于输入）。</summary>
        public string SortOrderText
        {
            get => _sortOrderText;
            set => SetProperty(ref _sortOrderText, value);
        }

        public string Type
        {
            get => TypeOptions[Math.Clamp(_type - 1, 0, TypeOptions.Length - 1)];
            set { var i = Array.IndexOf(TypeOptions, value); if (i >= 0) _type = i + 1; }
        }

        public string Status
        {
            get => _status == 1 ? StatusOptions[0] : StatusOptions[1];
            set { var i = Array.IndexOf(StatusOptions, value); if (i >= 0) _status = i == 0 ? 1 : 0; }
        }

        public bool IsNew => _isNew;

        /// <summary>构建上级组织选项：根级 + 按层级缩进的全部组织（编辑时排除自身及子孙）。</summary>
        private void BuildParentOptions(string? excludeId)
        {
            ParentOptions.Add(new ParentOption { Display = "根级（集团总部）", Id = null });

            var excluded = new HashSet<string>();
            if (!string.IsNullOrEmpty(excludeId))
            {
                excluded.Add(excludeId);
                void CollectDescendants(string pid)
                {
                    foreach (var o in _all.Where(x => x.ParentId == pid))
                    {
                        if (excluded.Add(o.Id)) CollectDescendants(o.Id);
                    }
                }
                CollectDescendants(excludeId);
            }

            foreach (var root in _all.Where(o => string.IsNullOrEmpty(o.ParentId))
                         .OrderBy(o => o.SortOrder).ThenBy(o => o.Id))
            {
                AddParentOption(root, 0, excluded);
            }
        }

        private void AddParentOption(Organization o, int depth, HashSet<string> excluded)
        {
            if (!excluded.Contains(o.Id))
            {
                ParentOptions.Add(new ParentOption { Display = new string('　', depth * 2) + o.Name, Id = o.Id });
            }
            foreach (var c in _all.Where(x => x.ParentId == o.Id)
                         .OrderBy(x => x.SortOrder).ThenBy(x => x.Id))
            {
                AddParentOption(c, depth + 1, excluded);
            }
        }

        private int SortValue => int.TryParse(SortOrderText?.Trim(), out var n) ? n : 0;

        public Organization BuildEntity() => new()
        {
            Name = Name.Trim(),
            Code = Code.Trim(),
            ParentId = _parentId,
            Type = _type,
            Manager = Manager.Trim(),
            Phone = Phone.Trim(),
            Email = Email.Trim(),
            SortOrder = SortValue,
            Status = _status,
            Description = Description.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(Organization org)
        {
            org.Name = Name.Trim();
            org.Code = Code.Trim();
            org.ParentId = _parentId;
            org.Type = _type;
            org.Manager = Manager.Trim();
            org.Phone = Phone.Trim();
            org.Email = Email.Trim();
            org.SortOrder = SortValue;
            org.Status = _status;
            org.Description = Description.Trim();
            org.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写组织名称。"; return false; }
            Message = "";
            return true;
        }
    }
}
