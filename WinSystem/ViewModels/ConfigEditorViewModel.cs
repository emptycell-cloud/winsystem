using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>系统参数编辑表单：新增/编辑系统参数（sys_config）。</summary>
    public class ConfigEditorViewModel : EditorFormViewModel
    {
        public static readonly string[] TypeOptions = { "字符串", "数字", "布尔", "JSON" };

        private bool _isNew;
        private int _isSystem;
        private string _configName;
        private string _configKey;
        private string _configValue;
        private string _remark;
        private int _configType;

        public ConfigEditorViewModel(SysConfig? cfg)
        {
            _isNew = cfg == null;
            _configName = cfg?.ConfigName ?? "";
            _configKey = cfg?.ConfigKey ?? "";
            _configValue = cfg?.ConfigValue ?? "";
            _remark = cfg?.Remark ?? "";
            _configType = cfg?.ConfigType ?? 1;
            _isSystem = cfg?.IsSystem ?? 0;
            Title = _isNew ? "新增系统参数" : "编辑系统参数";
        }

        public static ConfigEditorViewModel CreateNew() => new(null);

        public string ConfigName { get => _configName; set => SetProperty(ref _configName, value); }
        public string ConfigKey { get => _configKey; set => SetProperty(ref _configKey, value); }
        public string ConfigValue { get => _configValue; set => SetProperty(ref _configValue, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        public bool IsNew => _isNew;

        /// <summary>是否系统内置参数（编辑内置参数时展示来源提示）。</summary>
        public bool IsSystemBuiltIn => _isSystem == 1;

        public string ConfigType
        {
            get => TypeOptions[Math.Clamp(_configType - 1, 0, TypeOptions.Length - 1)];
            set { var i = Array.IndexOf(TypeOptions, value); if (i >= 0) _configType = i + 1; }
        }

        public SysConfig BuildEntity() => new()
        {
            ConfigName = ConfigName.Trim(),
            ConfigKey = ConfigKey.Trim(),
            ConfigValue = ConfigValue,
            ConfigType = _configType,
            IsSystem = _isSystem,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        public void ApplyTo(SysConfig cfg)
        {
            cfg.ConfigName = ConfigName.Trim();
            cfg.ConfigKey = ConfigKey.Trim();
            cfg.ConfigValue = ConfigValue;
            cfg.ConfigType = _configType;
            cfg.Remark = Remark.Trim();
            cfg.UpdatedAt = DateTime.Now;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(ConfigName)) { Message = "请填写参数名称。"; return false; }
            if (string.IsNullOrWhiteSpace(ConfigKey)) { Message = "请填写参数键。"; return false; }
            Message = "";
            return true;
        }
    }
}
