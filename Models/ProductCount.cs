namespace WebApplication1.Models
{
    public class ProductCount
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
    }
}

//Изменить на Cart с привязкой к пользователю. Связь многие ко многим.
