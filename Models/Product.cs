namespace WebApplication1.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }
    }
}
