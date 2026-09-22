using System.Collections.ObjectModel;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    public class OrderEditorViewModel : EditorFormViewModel
    {
        private string _orderNo;
        private string _customer;
        private string _productName;
        private string _quantity;
        private string _price;
        private string _status;
        private string _remark;

        public ObservableCollection<string> ProductNames { get; } = new();
        public ObservableCollection<string> StatusOptions { get; } = new();

        public OrderEditorViewModel(Order? order, List<Product> products)
        {
            foreach (var p in products.Where(p => p.Enabled)) ProductNames.Add(p.Name);
            foreach (var s in new[] { "待付款", "待发货", "处理中", "已完成", "已取消" }) StatusOptions.Add(s);

            _orderNo = order?.OrderNo ?? GenerateNo();
            _customer = order?.Customer ?? "";
            _productName = order?.ProductName ?? (ProductNames.FirstOrDefault() ?? "");
            _quantity = order is null ? "1" : order.Quantity.ToString();
            _price = order is null ? "" : order.UnitPrice.ToString("0.##");
            _status = order?.Status ?? "待付款";
            _remark = order?.Remark ?? "";
            Title = order == null ? "新增订单" : "编辑订单";
        }

        private static string GenerateNo() => "SO" + DateTime.Now.ToString("yyyyMMddHHmmss");

        public string OrderNo { get => _orderNo; set => SetProperty(ref _orderNo, value); }
        public string Customer { get => _customer; set => SetProperty(ref _customer, value); }
        public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }
        public string Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
        public string Price { get => _price; set => SetProperty(ref _price, value); }
        public string Status { get => _status; set => SetProperty(ref _status, value); }
        public string Remark { get => _remark; set => SetProperty(ref _remark, value); }

        public Order BuildEntity() => new()
        {
            OrderNo = OrderNo.Trim(),
            Customer = Customer.Trim(),
            ProductName = ProductName,
            Quantity = int.TryParse(Quantity, out var q) ? Math.Max(1, q) : 1,
            UnitPrice = decimal.TryParse(Price, out var p) ? p : 0,
            Status = Status,
            Remark = Remark.Trim(),
            CreatedAt = DateTime.Now
        };

        public void ApplyTo(Order order)
        {
            order.OrderNo = OrderNo.Trim();
            order.Customer = Customer.Trim();
            order.ProductName = ProductName;
            order.Quantity = int.TryParse(Quantity, out var q) ? Math.Max(1, q) : 1;
            order.UnitPrice = decimal.TryParse(Price, out var p) ? p : 0;
            order.Status = Status;
            order.Remark = Remark.Trim();
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Customer)) { Message = "请填写客户名称。"; return false; }
            if (string.IsNullOrWhiteSpace(ProductName)) { Message = "请选择商品。"; return false; }
            if (!int.TryParse(Quantity, out var q) || q <= 0) { Message = "数量必须是正整数。"; return false; }
            if (!decimal.TryParse(Price, out _) || Price is null) { Message = "单价必须是有效数字。"; return false; }
            Message = "";
            return true;
        }
    }
}