using FluentAssertions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.Repository.Default;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly ILogger<CartService> _logger;
        private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CartService(ILogger<CartService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdFull(userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            return _mapper.Map<CartDto>(cart);
        }
        public async Task RemoveFromCartAsync(Guid userId, Guid productId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdWithCartElements(userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            var cartElement = cart.CartElement.FirstOrDefault(ce => ce.Product.ProductId == productId);
            if (cartElement == null)
            {
                _logger.LogWarning("Элемент коризны {UserId} не найдена.", userId);
                throw new InvalidOperationException("Элемент корзины не найден.");
            }
            else
            {
                cart.CartElement.Remove(cartElement);
                _unitOfWork.CartElements.Delete(cartElement);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdWithCartElements(userId);

            if (cart == null)
            {
                _logger.LogWarning("Корзина для пользователя {UserId} не найдена.", userId);
                throw new InvalidOperationException("Корзина не найдена.");
            }

            cart.CartElement.Clear();
            await _unitOfWork.SaveChangesAsync();
        }

        // Обновление количества товара в корзине
        public async Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int change)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdFull(userId);

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
                    _unitOfWork.CartElements.Delete(cartElement);
                }
            }
            else
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);

                if(product == null)
                {
                    _logger.LogWarning("Продукт {productId} не найден.", productId);
                    throw new InvalidOperationException("Продукт не найден.");
                }

                CartElement newCartElem = new CartElement
                {
                    CartElementId = Guid.NewGuid(),
                    Product = product,
                    Count = change
                };
                cart.CartElement.Add(newCartElem);

                await _unitOfWork.CartElements.AddAsync(newCartElem);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // Оформление выбранных товаров
        public async Task<OrderDto> CheckoutSelectedAsync(Guid userId, List<Guid> productIds, Guid addressId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdFull(userId);

            var user = await _unitOfWork.Users.GetByIdWithOrders(userId);

            if (cart == null || !cart.CartElement.Any())
            {
                _logger.LogWarning("Корзина для пользователя {UserId} пуста.", userId);
                throw new InvalidOperationException("Корзина пуста.");
            }

            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);
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
                    AddressElementId = Guid.NewGuid(),
                    City = address.City,
                    Street = address.Street,
                    House = address.House,
                    Apartment = address.Apartment
                },
                OrderElement = selectedCartElements.Select(ce => new OrderElement
                {
                    OrderElementId = Guid.NewGuid(),
                    Product = ce.Product,
                    Count = ce.Count
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            user.Orders.Add(order);

            // Удаляем выбранные товары из корзины
            foreach (var cartElement in selectedCartElements)
            {
                cart.CartElement.Remove(cartElement);
                _unitOfWork.CartElements.Delete(cartElement);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }
    }
}
