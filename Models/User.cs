using WebApplication1.DataBase;

namespace WebApplication1.Models
{
    public class User
    {
        public User() { }

        public Guid UserId { get; set; }

        public string? Name { get; set; } = null;

        public List<Adress>? Adresses { get; set; } = new List<Adress>();

        public List<Order>? Orders { get; set; } = new List<Order>();

        public string? Phone { get; set; } = string.Empty;

        public Cart Cart { get; set; }
    }
}
