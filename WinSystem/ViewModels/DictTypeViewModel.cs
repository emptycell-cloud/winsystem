using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    /// <summary>数据字典页 ViewModel：左侧字典类型列表 + 右侧选中类型的字典数据（sys_dict_type / sys_dict_data）。</summary>
    public class DictTypeViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private string _searchText = "";
        private SysDictType? _selectedType;
        private bool _isLoading;
        private bool _dataLoaded;

        public DictTypeViewModel()
        {
            _data = Services.Session.Data;
            SearchCommand = new RelayCommand(_ => RefreshTypes());
            AddTypeCommand = new RelayCommand(async _ => await AddType());
            EditTypeCommand = new RelayCommand(async o => await EditType(o as SysDictType));
            DeleteTypeCommand = new RelayCommand(async o => await DeleteType(o as SysDictType));
            AddDataCommand = new RelayCommand(async _ => await AddData());
            EditDataCommand = new RelayCommand(async o => await EditData(o as SysDictData));
            DeleteDataCommand = new RelayCommand(async o => await DeleteData(o as SysDictData));
        }

        public ObservableCollection<SysDictType> TypeItems { get; } = new();
        public ObservableCollection<SysDictData> DataItems { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public SysDictType? SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    if (value != null) _ = LoadDataAsync(value.Code);
                }
            }
        }

        /// <summary>右侧字典数据加载动画层。</summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand AddTypeCommand { get; }
        public ICommand EditTypeCommand { get; }
        public ICommand DeleteTypeCommand { get; }
        public ICommand AddDataCommand { get; }
        public ICommand EditDataCommand { get; }
        public ICommand DeleteDataCommand { get; }

        /// <summary>页面激活时调用：字典类型仅首次进入时加载一次。</summary>
        public async Task ReloadAsync()
        {
            if (!_dataLoaded) await LoadCoreAsync();
        }

        private async Task LoadCoreAsync(bool refresh = false)
        {
            if (refresh || !_dataLoaded) await _data.LoadDictTypesAsync();
            _dataLoaded = true;
            RefreshTypes();
            if (TypeItems.Count > 0 && SelectedType == null)
            {
                SelectedType = TypeItems[0];
            }
        }

        /// <summary>按查询关键字过滤并重建类型列表，尽量保留当前选中项。</summary>
        private void RefreshTypes()
        {
            var kw = _searchText?.Trim().ToLower() ?? "";
            var list = string.IsNullOrEmpty(kw)
                ? _data.DictTypes.ToList()
                : _data.DictTypes.Where(t =>
                      (t.Name ?? "").ToLower().Contains(kw) ||
                      (t.Code ?? "").ToLower().Contains(kw)).ToList();

            var sel = SelectedType;
            TypeItems.Clear();
            foreach (var t in list) TypeItems.Add(t);

            if (sel != null)
            {
                SelectedType = TypeItems.FirstOrDefault(t => t.Id == sel.Id);
                if (SelectedType == null && TypeItems.Count > 0) SelectedType = TypeItems[0];
            }
            else if (TypeItems.Count > 0)
            {
                SelectedType = TypeItems[0];
            }
        }

        private async Task LoadDataAsync(string code)
        {
            IsLoading = true;
            try
            {
                var list = await _data.LoadDictDataAsync(code);
                DataItems.Clear();
                foreach (var d in list) DataItems.Add(d);
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ================= 类型操作 =================

        private async Task AddType()
        {
            var editor = DictTypeEditorViewModel.CreateNew();
            if (Views.DialogService.ShowDictTypeEditor(editor) is true)
            {
                var type = editor.BuildEntity();
                await _data.AddDictTypeAsync(type);
                await LoadCoreAsync(refresh: true);
                SelectedType = TypeItems.FirstOrDefault(t => t.Id == type.Id);
                Views.DialogService.Success($"字典类型“{type.Name}”新增成功。");
            }
        }

        private async Task EditType(SysDictType? type)
        {
            type ??= SelectedType;
            if (type == null) return;
            var editor = new DictTypeEditorViewModel(type);
            if (Views.DialogService.ShowDictTypeEditor(editor) is true)
            {
                editor.ApplyTo(type);
                var code = type.Code;
                await _data.UpdateDictTypeAsync(type);
                await LoadCoreAsync(refresh: true);
                SelectedType = TypeItems.FirstOrDefault(t => t.Code == code);
                Views.DialogService.Success($"字典类型“{type.Name}”修改成功。");
            }
        }

        private async Task DeleteType(SysDictType? type)
        {
            type ??= SelectedType;
            if (type is not SysDictType target) return;
            if (!Views.DialogService.Confirm($"确定要删除字典类型“{target.Name}”及其全部字典数据吗？"))
                return;
            try
            {
                await _data.DeleteDictTypeAsync(target);
                DataItems.Clear();
                await LoadCoreAsync(refresh: true);
                Views.DialogService.Success($"字典类型“{target.Name}”已删除。");
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }

        // ================= 字典数据操作 =================

        private async Task AddData()
        {
            if (SelectedType == null)
            {
                Views.DialogService.Info("请先选择左侧字典类型。");
                return;
            }
            var editor = DictDataEditorViewModel.CreateNew(SelectedType.Code);
            if (Views.DialogService.ShowDictDataEditor(editor) is true)
            {
                var data = editor.BuildEntity();
                var created = await _data.AddDictDataAsync(data);
                DataItems.Add(created);
                SelectedType.DataCount++;
                RefreshTypes();
                Views.DialogService.Success($"字典数据“{created.Label}”新增成功。");
            }
        }

        private async Task EditData(SysDictData? data)
        {
            if (data is not SysDictData target) return;
            var editor = new DictDataEditorViewModel(target);
            if (Views.DialogService.ShowDictDataEditor(editor) is true)
            {
                editor.ApplyTo(target);
                await _data.UpdateDictDataAsync(target);
                // 重新加载列表，确保界面显示最新数据
                if (SelectedType != null) await LoadDataAsync(SelectedType.Code);
                Views.DialogService.Success($"字典数据“{target.Label}”修改成功。");
            }
        }

        private async Task DeleteData(SysDictData? data)
        {
            if (data is not SysDictData target) return;
            if (!Views.DialogService.Confirm($"确定要删除字典数据“{target.Label}”吗？"))
                return;
            try
            {
                await _data.DeleteDictDataAsync(target);
                DataItems.Remove(target);
                if (SelectedType != null) SelectedType.DataCount--;
                RefreshTypes();
                Views.DialogService.Success($"字典数据“{target.Label}”已删除。");
            }
            catch (Exception ex)
            {
                Views.DialogService.Error(ex.Message);
            }
        }
    }
}
