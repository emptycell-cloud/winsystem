namespace WinSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = "";
        public string Customer { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Amount => Quantity * UnitPrice;
        public string Status { get; set; } = "待处理";
        public string Remark { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}