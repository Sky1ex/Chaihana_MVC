namespace WebApplication1.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
    }
}
