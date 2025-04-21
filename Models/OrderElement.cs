namespace WebApplication1.Models
{
    public class OrderElement
    {
        public Guid OrderElementId { get; set; } = Guid.NewGuid();
        public required Product Product { get; set; }
        public int Count { get; set; } = 1;
    }
}
