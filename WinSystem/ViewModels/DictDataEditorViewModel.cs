using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>字典数据编辑表单：新增/编辑字典数据（sys_dict_data）。</summary>
    public class DictDataEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] StatusOptions = { "启用", "停用" };

        private bool _isNew;
        private readonly string _dictCode;
        private string _label;
        private string _value;
        private string _remark;
        private string _sortOrderText;
        private int _status;

        /// <summary>新增：指定所属字典编码。</summary>
        public DictDataEditorViewModel(string dictCode) : this((SysDictData?)null) => _dictCode = dictCode;

        public DictDataEditorViewModel(SysDictData? data)
        {
            _isNew = data == null;
            _dictCode = data?.DictCode ?? "";
            _label = data?.Label ?? "";
            _value = data?.Value ?? "";
            _remark = data?.Remark ?? "";
            _sortOrderText = (data?.SortOrder ?? 0).ToString();
            _status = data?.Status ?? 1;
            Title = _isNew ? "新增字典数据" : "编辑字典数据";
        }

        public static DictDataEditorViewModel CreateNew(string dictCode) => new(dictCode);

        /// <summary>所属字典编码（只读展示）。</summary>
        public string DictCode => _dictCode;

        public string Label { get => _label; set => SetProperty(ref _label, value); }
        public string Value { get => _value; set => SetProperty(ref _value, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }
        public string SortOrderText { get => _sortOrderText; set => SetProperty(ref _sortOrderText, value); }

        public string Status
        {
            get => StatusOptions[_status == 1 ? 0 : 1];
            set { var i = Array.IndexOf(StatusOptions, value); if (i >= 0) _status = i == 0 ? 1 : 0; }
        }

        public bool IsNew => _isNew;

        private int SortValue => int.TryParse(SortOrderText?.Trim(), out var n) ? n : 0;

        public SysDictData BuildEntity() => new()
        {
            DictCode = _dictCode,
            Label = Label.Trim(),
            Value = Value.Trim(),
            Status = _status,
            SortOrder = SortValue,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(SysDictData data)
        {
            data.Label = Label.Trim();
            data.Value = Value.Trim();
            data.Status = _status;
            data.SortOrder = SortValue;
            data.Remark = Remark.Trim();
            data.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Label)) { Message = "请填写数据标签。"; return false; }
            if (string.IsNullOrWhiteSpace(Value)) { Message = "请填写数据键值。"; return false; }
            Message = "";
            return true;
        }
    }
}
