using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(ce => ce.Product)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                return null;
            }

            return new CartDto
            {
                CartId = cart.CartId,
                Products = cart.CartElement.Select(ce => new CartProductDto
                {
                    ProductId = ce.Product.ProductId,
                    Name = ce.Product.Name,
                    Price = ce.Product.Price,
                    Count = ce.Count,
                    ImageUrl = ce.Product.ImageUrl
                }).ToList()
            };
        }
        public async Task RemoveFromCartAsync(Guid userId, Guid productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            var cartElement = cart.CartElement.FirstOrDefault(ce => ce.Product.ProductId == productId);
            if (cartElement != null)
            {
                cart.CartElement.Remove(cartElement);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            cart.CartElement.Clear();
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDto> CheckoutAsync(Guid userId, Guid addressId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(ce => ce.Product)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            if (cart == null || !cart.CartElement.Any())
            {
                _logger.LogWarning("Корзина для пользователя {UserId} пуста.", userId);
                throw new InvalidOperationException("Корзина пуста.");
            }

            var address = await _context.Adresses.FindAsync(addressId);
            if (address == null)
            {
                _logger.LogWarning("Адрес {AddressId} не найден.", addressId);
                throw new InvalidOperationException("Адрес не найден.");
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                dateTime = DateTimeOffset.UtcNow,
                Adress = new AddressElement { 
                    AddressElementId = address.AdressId,
                    City = address.City,
                    Street = address.Street,
                    House = address.House
                },
                OrderElement = cart.CartElement.Select(ce => new OrderElement
                {
                    OrderElementId = Guid.NewGuid(),
                    Product = ce.Product,
                    Count = ce.Count
                }).ToList()
            };

            _context.Orders.Add(order);
            cart.CartElement.Clear();
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                DateTime = order.dateTime,
                Address = new AddressDto
                {
                    City = address.City,
                    Street = address.Street,
                    House = address.House
                },
                Products = order.OrderElement.Select(oe => new OrderProductDto
                {
                    ProductId = oe.Product.ProductId,
                    Name = oe.Product.Name,
                    Price = oe.Product.Price,
                    Count = oe.Count
                }).ToList()
            };
        }

        // Обновление количества товара в корзине
        public async Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int change)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            var cartElement = cart.CartElement.FirstOrDefault(ce => ce.Product.ProductId == productId);
            if (cartElement != null)
            {
                cartElement.Count += change;

                if (cartElement.Count <= 0)
                {
                    cart.CartElement.Remove(cartElement);
                    _context.CartElements.Remove(cartElement);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                CartElement newCartElem = new CartElement
                {
                    CartElementId = Guid.NewGuid(),
                    Product = product,
                    Count = change
                };
                cart.CartElement.Add(newCartElem);

                _context.CartElements.Add(newCartElem);
                await _context.SaveChangesAsync();
            }
        }

        // Оформление выбранных товаров
        public async Task<OrderDto> CheckoutSelectedAsync(Guid userId, List<Guid> productIds, Guid addressId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(ce => ce.Product)
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            var user = await _context.Users
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartElement.Any())
            {
                _logger.LogWarning("Корзина для пользователя {UserId} пуста.", userId);
                throw new InvalidOperationException("Корзина пуста.");
            }

            var address = await _context.Adresses.FindAsync(addressId);
            if (address == null)
            {
                _logger.LogWarning("Адрес {AddressId} не найден.", addressId);
                throw new InvalidOperationException("Адрес не найден.");
            }

            var selectedCartElements = cart.CartElement
                .Where(ce => productIds.Contains(ce.Product.ProductId))
                .ToList();

            if (!selectedCartElements.Any())
            {
                _logger.LogWarning("Выбранные товары не найдены в корзине.");
                throw new InvalidOperationException("Товары не найдены.");
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                dateTime = DateTimeOffset.UtcNow,
                Adress = new AddressElement
                {
                    AddressElementId = address.AdressId,
                    City = address.City,
                    Street = address.Street,
                    House = address.House
                },
                OrderElement = selectedCartElements.Select(ce => new OrderElement
                {
                    OrderElementId = Guid.NewGuid(),
                    Product = ce.Product,
                    Count = ce.Count
                }).ToList()
            };

            _context.Orders.Add(order);
            user.Orders.Add(order);

            // Удаляем выбранные товары из корзины
            foreach (var cartElement in selectedCartElements)
            {
                cart.CartElement.Remove(cartElement);
            }

            await _context.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                DateTime = order.dateTime,
                Address = new AddressDto
                {
                    City = address.City,
                    Street = address.Street,
                    House = address.House
                },
                Products = order.OrderElement.Select(oe => new OrderProductDto
                {
                    ProductId = oe.Product.ProductId,
                    Name = oe.Product.Name,
                    Price = oe.Product.Price,
                    Count = oe.Count
                }).ToList()
            };
        }

        // Получение адресов пользователя
        public async Task<List<AddressDto>> GetUserAddressesAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            List<Models.Adress> addresses = user.Adresses;

            List<AddressDto> ad = addresses.Select(a => new AddressDto
            {
                AddressId = a.AdressId,
                City = a.City,
                Street = a.Street,
                House = a.House
            })
                .ToList();

            return ad;
        }
    }
}
