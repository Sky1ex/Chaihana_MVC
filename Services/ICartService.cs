using WebApplication1.DTO;

namespace WebApplication1.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, Guid productId, int count);
        Task RemoveFromCartAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);
        Task<OrderDto> CheckoutAsync(Guid userId, Guid addressId);

        Task<OrderDto> CheckoutSelectedAsync(Guid userId, List<Guid> productIds, Guid addressId);
        Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int change);
        Task<List<AddressDto>> GetUserAddressesAsync(Guid userId);
        /*Task PurshareAsync(Guid _userId, string _address);*/
    }
}
