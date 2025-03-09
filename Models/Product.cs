namespace WebApplication1.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = null;
        public required decimal Price { get; set; }
        public required double Weight { get; set; }
        public required string ImageUrl { get; set; }
        public Category Category { get; set; } = null;
    }
}
