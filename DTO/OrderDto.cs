namespace WebApplication1.DTO
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public AddressDto Address { get; set; }
        public List<OrderElementDto> Products { get; set; }
    }
}
