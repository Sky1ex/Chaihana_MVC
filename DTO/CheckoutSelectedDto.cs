namespace WebApplication1.DTO
{
    public class CheckoutSelectedDto
    {
        public List<Guid> ProductIds { get; set; }
        public Guid AddressId { get; set; }
    }
}