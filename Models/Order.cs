using System.Net;

namespace WebApplication1.Models
{
    public class Order
    {
        public required DateTimeOffset dateTime { get; set; }

        public Guid OrderId { get; set; }

        public  List<OrderElement> OrderElement { get; set; } = new List<OrderElement>();

        public required AddressElement Adress { get; set; }

        public int Sum { get; set; } = 0;
    }
}
