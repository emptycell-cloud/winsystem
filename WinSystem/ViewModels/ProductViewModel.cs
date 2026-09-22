using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private string _searchText = "";
        private string _categoryFilter = "全部";
        private Product? _selected;

        public System.Collections.ObjectModel.ObservableCollection<string> Categories { get; } = new();

        public ProductViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as Product));
            DeleteCommand = new RelayCommand(async o => await Delete(o as Product));
            ToggleStatusCommand = new RelayCommand(async o => await ToggleStatus(o as Product));
            RefreshCategories();
            Refresh();
        }

        public ObservableCollection<Product> Items { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) Refresh(); }
        }

        public string CategoryFilter
        {
            get => _categoryFilter;
            set { if (SetProperty(ref _categoryFilter, value)) Refresh(); }
        }

        public Product? Selected
        {
            get => _selected;
            set { SetProperty(ref _selected, value); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ToggleStatusCommand { get; }

        private void RefreshCategories()
        {
            var was = Categories.Contains(CategoryFilter) || CategoryFilter == "全部";
            Categories.Clear();
            Categories.Add("全部");
            foreach (var c in _data.Products.Select(p => p.Category).Distinct().OrderBy(c => c))
                Categories.Add(c);
            if (!was) CategoryFilter = "全部";
        }

        private void Refresh()
        {
            Items.Clear();
            var q = _searchText?.Trim().ToLower() ?? "";
            var list = _data.Products.AsEnumerable();
            if (!string.IsNullOrEmpty(q))
                list = list.Where(p =>
                    p.Name.ToLower().Contains(q) ||
                    p.Category.ToLower().Contains(q) ||
                    p.Description.ToLower().Contains(q));
            if (CategoryFilter != "全部")
                list = list.Where(p => p.Category == CategoryFilter);

            foreach (var p in list.OrderBy(p => p.Id)) Items.Add(p);
        }

        private async Task Add()
        {
            var editor = new ProductEditorViewModel(null);
            if (Views.DialogService.ShowProductEditor(editor) is true)
            {
                var product = editor.BuildEntity();
                await _data.AddProductAsync(product);
                RefreshCategories();
                Refresh();
            }
        }

        private async Task Edit(Product? product)
        {
            product ??= Selected;
            if (product == null) return;
            var editor = new ProductEditorViewModel(product);
            if (Views.DialogService.ShowProductEditor(editor) is true)
            {
                editor.ApplyTo(product);
                await _data.UpdateProductAsync(product);
                RefreshCategories();
                Refresh();
            }
        }

        private async Task Delete(Product? product)
        {
            product ??= Selected;
            if (product is not Product p) return;
            if (Views.DialogService.Confirm($"确定要删除商品“{p.Name}”吗？"))
            {
                await _data.DeleteProductAsync(p);
                RefreshCategories();
                Refresh();
            }
        }

        private async Task ToggleStatus(Product? product)
        {
            product ??= Selected;
            if (product is not Product p) return;
            p.Enabled = !p.Enabled;
            await _data.UpdateProductAsync(p);
            Refresh();
        }
    }
}