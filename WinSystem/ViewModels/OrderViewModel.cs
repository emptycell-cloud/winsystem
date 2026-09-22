using System.Collections.ObjectModel;
using System.Windows.Input;
using WinSystem.Commands;
using WinSystem.Models;

namespace WinSystem.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly Services.DataService _data;
        private string _searchText = "";
        private string _statusFilter = "全部";
        private Order? _selected;

        public System.Collections.ObjectModel.ObservableCollection<string> StatusOptions { get; } = new();

        public OrderViewModel()
        {
            _data = Services.Session.Data;
            AddCommand = new RelayCommand(async _ => await Add());
            EditCommand = new RelayCommand(async o => await Edit(o as Order));
            DeleteCommand = new RelayCommand(async o => await Delete(o as Order));
            AdvanceCommand = new RelayCommand(async o => await Advance(o as Order));
            StatusOptions.Add("全部");
            foreach (var s in new[] { "待付款", "待发货", "处理中", "已完成", "已取消" }) StatusOptions.Add(s);
            Refresh();
        }

        public ObservableCollection<Order> Items { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) Refresh(); }
        }

        public string StatusFilter
        {
            get => _statusFilter;
            set { if (SetProperty(ref _statusFilter, value)) Refresh(); }
        }

        public Order? Selected
        {
            get => _selected;
            set { SetProperty(ref _selected, value); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AdvanceCommand { get; }

        private void Refresh()
        {
            Items.Clear();
            var q = _searchText?.Trim().ToLower() ?? "";
            var list = _data.Orders.AsEnumerable();
            if (!string.IsNullOrEmpty(q))
            {
                var words = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                list = list.Where(o =>
                    words.Any(w =>
                        o.OrderNo.ToLower().Contains(w) ||
                        o.Customer.ToLower().Contains(w) ||
                        o.ProductName.ToLower().Contains(w)));
            }
            if (StatusFilter != "全部")
                list = list.Where(o => o.Status == StatusFilter);

            foreach (var o in list.OrderByDescending(o => o.CreatedAt)) Items.Add(o);
        }

        private async Task Add()
        {
            var editor = new OrderEditorViewModel(null, _data.Products.ToList());
            if (Views.DialogService.ShowOrderEditor(editor) is true)
            {
                var order = editor.BuildEntity();
                await _data.AddOrderAsync(order);
                Refresh();
            }
        }

        private async Task Edit(Order? order)
        {
            order ??= Selected;
            if (order == null) return;
            var editor = new OrderEditorViewModel(order, _data.Products.ToList());
            if (Views.DialogService.ShowOrderEditor(editor) is true)
            {
                editor.ApplyTo(order);
                await _data.UpdateOrderAsync(order);
                Refresh();
            }
        }

        private async Task Delete(Order? order)
        {
            order ??= Selected;
            if (order is not Order o) return;
            if (Views.DialogService.Confirm($"确定要删除订单“{o.OrderNo}”吗？"))
            {
                await _data.DeleteOrderAsync(o);
                Refresh();
            }
        }

        private async Task Advance(Order? order)
        {
            order ??= Selected;
            if (order is not Order o) return;
            var next = new Dictionary<string, string>
            {
                ["待付款"] = "待发货",
                ["待发货"] = "处理中",
                ["处理中"] = "已完成"
            };
            if (next.TryGetValue(o.Status, out var s))
            {
                o.Status = s;
                await _data.UpdateOrderAsync(o);
                Refresh();
            }
        }
    }
}