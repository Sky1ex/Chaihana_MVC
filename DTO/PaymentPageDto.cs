using WebApplication1.Models;

namespace WebApplication1.DTO
{
    public class PaymentPageDto
    {
        public List<CartProductDto> Products { get; set; }

        public Adress Address { get; set; }
    }
}
