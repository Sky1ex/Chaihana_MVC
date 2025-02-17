namespace WebApplication1.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public List<Adress> Adresses { get; set; }

        public List<Order> Orders { get; set; }

        public string Phone { get; set; }

        public  Cart Cart { get; set; }
    }
}
