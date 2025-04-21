using WebApplication1.Models;

namespace WebApplication1.Repository.Default
{
    public interface IUnitOfWork : IDisposable
    {
        CartRepository Carts { get; }
        AddressRepository Addresses { get; }
        OrderRepository Orders { get; }
        CartElementRepository CartElements { get; }
        UserRepository Users { get; }
        ProductRepository Products { get; }
        CategoryRepository Categories { get; }
        BookingRepository Bookings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
