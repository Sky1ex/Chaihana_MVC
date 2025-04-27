using WebApplication1.DTO;

namespace WebApplication1.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid userId);
        Task RemoveFromCartAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);

        Task<OrderDto> CheckoutSelectedAsync(Guid userId, List<Guid> productIds, Guid addressId);
        Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int change);
    }
}
