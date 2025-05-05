namespace WebApplication1.Models
{
    public class Adress
    {
        public Guid AdressId { get; set; } = Guid.NewGuid();
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string House { get; set; }
        public required int Apartment { get; set; }
	}
}
