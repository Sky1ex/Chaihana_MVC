namespace WebApplication1.Models
{
    public class OrderElement
    {
        public Guid OrderElementId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
