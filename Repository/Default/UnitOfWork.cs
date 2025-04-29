using WebApplication1.DataBase;
using WebApplication1.Models;

namespace WebApplication1.Repository.Default
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Carts = new CartRepository(_context);
            Addresses = new AddressRepository(_context);
            Orders = new OrderRepository(_context);
            CartElements = new CartElementRepository(_context);
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Bookings = new BookingRepository(_context);
        }

        public CartRepository Carts { get; }
        public AddressRepository Addresses { get; }
        public OrderRepository Orders { get; }
        public CartElementRepository CartElements { get; }
        public UserRepository Users { get; }
        public ProductRepository Products { get; }
        public CategoryRepository Categories { get; }
        public BookingRepository Bookings { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
