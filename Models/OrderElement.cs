namespace WebApplication1.Models
{
    public class OrderElement
    {
        public Guid OrderElementId { get; set; }
        public required Product Product { get; set; }
        public required int Count { get; set; } = 0;
    }
}
