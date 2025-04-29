using WebApplication1.DTO;

namespace WebApplication1.Services
{
    public interface IAccountService
    {
        Task<List<AddressDto>> GetAddresses(Guid userId);

        Task<List<OrderDto>> GetOrders(Guid userId);
    }
}
