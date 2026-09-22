using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>字典类型编辑表单：新增/编辑字典类型（sys_dict_type）。</summary>
    public class DictTypeEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] StatusOptions = { "启用", "停用" };

        private bool _isNew;
        private string _name;
        private string _code;
        private string _remark;
        private string _sortOrderText;
        private int _status;

        public DictTypeEditorViewModel(SysDictType? type)
        {
            _isNew = type == null;
            _name = type?.Name ?? "";
            _code = type?.Code ?? "";
            _remark = type?.Remark ?? "";
            _sortOrderText = (type?.SortOrder ?? 0).ToString();
            _status = type?.Status ?? 1;
            Title = _isNew ? "新增字典类型" : "编辑字典类型";
        }

        public static DictTypeEditorViewModel CreateNew() => new(null);

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Code { get => _code; set => SetProperty(ref _code, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }
        public string SortOrderText { get => _sortOrderText; set => SetProperty(ref _sortOrderText, value); }

        public string Status
        {
            get => StatusOptions[_status == 1 ? 0 : 1];
            set { var i = Array.IndexOf(StatusOptions, value); if (i >= 0) _status = i == 0 ? 1 : 0; }
        }

        public bool IsNew => _isNew;

        private int SortValue => int.TryParse(SortOrderText?.Trim(), out var n) ? n : 0;

        public SysDictType BuildEntity() => new()
        {
            Name = Name.Trim(),
            Code = Code.Trim(),
            Status = _status,
            SortOrder = SortValue,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(SysDictType type)
        {
            type.Name = Name.Trim();
            type.Code = Code.Trim();
            type.Status = _status;
            type.SortOrder = SortValue;
            type.Remark = Remark.Trim();
            type.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写字典名称。"; return false; }
            if (string.IsNullOrWhiteSpace(Code)) { Message = "请填写字典编码。"; return false; }
            Message = "";
            return true;
        }
    }
}
