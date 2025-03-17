using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;

namespace WebApplication1.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public AccountService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<AddressDto> GetAddresses(Guid userId)
        {
            var user = _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId).Result;
            List<AddressDto> Adresses = user.Adresses
                .Select(c => new AddressDto
                {
                    AddressId = c.AdressId,
                    City = c.City,
                    Street = c.Street,
                    House = c.House
                }).ToList();

            return Adresses;
        }

        public List<OrderDto> GetOrders(Guid userId)
        {
            var user = _context.Users
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.OrderElement)
                .ThenInclude(ced => ced.Product)
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.Adress)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var orders = user.Result.Orders
                .Select(c => new OrderDto
                {
                    DateTime = c.dateTime,
                    Address = new AddressDto
                    {
                        City = c.Adress.City,
                        Street = c.Adress.Street,
                        House = c.Adress.House
                    },
                    Products = c.OrderElement.Select(ce => new OrderProductDto
                    {
                        Name = ce.Product.Name,
                        Price = ce.Product.Price,
                        Count = ce.Count
                    }).ToList()
                })
                .ToList();

            return orders;
        }
    }
}
