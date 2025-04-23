namespace WebApplication1.DTO
{
    public class OrderElementDto
    {
        public Guid ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}
