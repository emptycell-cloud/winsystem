using WinSystem.Models;

namespace WinSystem.ViewModels
{
    public class ProductEditorViewModel : EditorFormViewModel
    {
        private string _name;
        private string _category;
        private string _price;
        private string _stock;
        private string _unit;
        private string _description;
        private bool _enabled;

        public static readonly string[] Categories = { "数码产品", "智能穿戴", "配件", "办公设备" };
        private readonly bool _isNew;

        public ProductEditorViewModel(Product? product)
        {
            _isNew = product == null;
            _name = product?.Name ?? "";
            _category = product?.Category ?? Categories[0];
            _price = product is null ? "" : product.Price.ToString("0.##");
            _stock = product is null ? "" : product.Stock.ToString();
            _unit = product?.Unit ?? "件";
            _description = product?.Description ?? "";
            _enabled = product?.Enabled ?? true;
            Title = _isNew ? "新增商品" : "编辑商品";
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Category { get => _category; set => SetProperty(ref _category, value); }
        public string Price { get => _price; set => SetProperty(ref _price, value); }
        public string Stock { get => _stock; set => SetProperty(ref _stock, value); }
        public string Unit { get => _unit; set => SetProperty(ref _unit, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public bool Enabled { get => _enabled; set => SetProperty(ref _enabled, value); }
        public bool IsEnabledReadOnly => !_isNew;

        public Product BuildEntity() => new()
        {
            Name = Name.Trim(),
            Category = Category,
            Price = decimal.TryParse(Price, out var p) ? p : 0,
            Stock = int.TryParse(Stock, out var s) ? s : 0,
            Unit = Unit.Trim(),
            Description = Description.Trim(),
            Enabled = Enabled,
            CreatedAt = DateTime.Now
        };

        public void ApplyTo(Product product)
        {
            product.Name = Name.Trim();
            product.Category = Category;
            product.Price = decimal.TryParse(Price, out var p) ? p : 0;
            product.Stock = int.TryParse(Stock, out var s) ? s : 0;
            product.Unit = Unit.Trim();
            product.Description = Description.Trim();
            product.Enabled = Enabled;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) { Message = "请填写商品名称。"; return false; }
            if (!decimal.TryParse(Price, out var price) || price < 0) { Message = "单价必须是有效数字。"; return false; }
            if (!int.TryParse(Stock, out _) || Stock is null) { Message = "库存必须是整数。"; return false; }
            Message = "";
            return true;
        }
    }
}