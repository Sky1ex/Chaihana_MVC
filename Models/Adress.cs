namespace WebApplication1.Models
{
    public class Adress
    {
        public Guid AdressId { get; set; }
        public required string City { get; set; } = "Nan";

        public required string Street { get; set; } = "Nan";

        public required string House { get; set; } = "Nan";
    }
}
