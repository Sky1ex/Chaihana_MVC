namespace WebApplication1.DTO
{
    public class CartElementDto
    {
        public Guid ProductId { get; set; }
        public ProductDto product { get; set; }
        public int Count { get; set; }
    }
}
