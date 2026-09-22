namespace WinSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Unit { get; set; } = "件";
        public bool Enabled { get; set; } = true;
        public string Status => Enabled ? "在售" : "下架";
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}