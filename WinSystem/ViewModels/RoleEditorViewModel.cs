using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>角色编辑表单：新增/编辑角色（sys_role）。</summary>
    public class RoleEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] DataScopeOptions = { "全部", "本部门及以下", "本部门", "仅本人", "自定义" };
        public static readonly string[] StatusOptions = { "启用", "停用" };

        private bool _isNew;
        private string _name;
        private string _code;
        private string _description;
        private string _remark;
        private string _sortOrderText;
        private int _dataScope;
        private int _status;

        public RoleEditorViewModel(Role? role)
        {
            _isNew = role == null;
            _name = role?.Name ?? "";
            _code = role?.Code ?? "";
            _description = role?.Description ?? "";
            _remark = role?.Remark ?? "";
            _sortOrderText = (role?.SortOrder ?? 0).ToString();
            _dataScope = role?.DataScope ?? 2;
            _status = role?.Status ?? 1;
            Title = _isNew ? "新增角色" : "编辑角色";
        }

        public static RoleEditorViewModel CreateNew() => new(null);

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Code { get => _code; set => SetProperty(ref _code, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }
        public string SortOrderText { get => _sortOrderText; set => SetProperty(ref _sortOrderText, value); }

        /// <summary>数据范围：index+1（1..5）。</summary>
        public string DataScope
        {
            get => DataScopeOptions[Math.Clamp(_dataScope - 1, 0, DataScopeOptions.Length - 1)];
            set { var i = Array.IndexOf(DataScopeOptions, value); if (i >= 0) _dataScope = i + 1; }
        }

        public string Status
        {
            get => StatusOptions[_status == 1 ? 0 : 1];
            set { var i = Array.IndexOf(StatusOptions, value); if (i >= 0) _status = i == 0 ? 1 : 0; }
        }

        public bool IsNew => _isNew;

        private int SortValue => int.TryParse(SortOrderText?.Trim(), out var n) ? n : 0;

        public Role BuildEntity() => new()
        {
            Name = Name.Trim(),
            Code = Code.Trim(),
            Description = Description.Trim(),
            DataScope = _dataScope,
            Status = _status,
            SortOrder = SortValue,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(Role role)
        {
            role.Name = Name.Trim();
            role.Code = Code.Trim();
            role.Description = Description.Trim();
            role.DataScope = _dataScope;
            role.Status = _status;
            role.SortOrder = SortValue;
            role.Remark = Remark.Trim();
            role.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写角色名称。"; return false; }
            if (string.IsNullOrWhiteSpace(Code)) { Message = "请填写角色编码。"; return false; }
            Message = "";
            return true;
        }
    }
}