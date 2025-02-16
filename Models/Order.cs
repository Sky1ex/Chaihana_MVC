namespace WebApplication1.Models
{
    public class Order
    {
        public DateTime dateTime { get; set; }

        public int OrderId { get; set; } // Добавить еще элементы...

        public List<ProductCount> ProductCounts { get; set; }
    }
}
