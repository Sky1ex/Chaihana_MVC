using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Cart
    {
        public Cart() { }

        [Key]
        [ForeignKey("User")]
        public Guid CartId { get; set; }
        public required User User { get; set; }
        public List<CartElement>? CartElement { get; set; } = new List<CartElement>();

    }
}

// Добавить такой же для Order для сохранения корзины.
