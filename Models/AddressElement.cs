namespace WebApplication1.Models
{
    public class AddressElement
    {
        public Guid AddressElementId { get; set; }
        public required string City { get; set; } = "Nan";

        public required string Street { get; set; } = "Nan";

        public required string House { get; set; } = "Nan";
    }
}
