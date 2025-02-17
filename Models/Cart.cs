using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Cart
    {
        [Key]
        [ForeignKey("User")]
        public Guid CartId { get; set; }
        public User User { get; set; }
        public List<CartElement> CartElement { get; set; }

    }
}

// Добавить такой же для Order для сохранения корзины.
