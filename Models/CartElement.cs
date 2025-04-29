namespace WebApplication1.Models
{
    public class CartElement
    {
        public Guid CartElementId { get; set; } = Guid.NewGuid();
        public required Product Product { get; set; }
        public int Count { get; set; } = 1;
    }
}

//Изменить на Cart с привязкой к пользователю. Связь один ко многим - Корзина к пользователю. При покупке заносить данные в Order и очищать корзину. Добавить к БД.