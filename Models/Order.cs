using System.Net;

namespace WebApplication1.Models
{
    public class Order
    {
        public DateTime dateTime { get; set; }

        public Guid OrderId { get; set; }

        public List<OrderElement> OrderElement { get; set; }

        public Adress Adress { get; set; }

        public int Sum { get; set; }
    }
}
