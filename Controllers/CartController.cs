using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.Controllers;
using WebApplication1.OtherClasses;
using System.Net.Http.Json;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _context;

        private readonly ILogger<CartController> _logger;

        private readonly UserService _userService;
        public CartController(ILogger<CartController> logger, ApplicationDbContext context, UserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            // Получаем текущего пользователя (если есть аутентификация)
            var userId = await _userService.AutoLogin();


            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(ce => ce.Product) // Загружаем Product для каждого CartElement
                .FirstOrDefaultAsync(c => c.User.UserId == userId);

            // Проверяем, есть ли товар в корзине
            var cartElement = cart.CartElement.FirstOrDefault(ce => ce.Product.ProductId == request.ProductId);

            if (cartElement != null)
            {
                // Если товар уже есть в корзине, увеличиваем количество
                cartElement.Count += request.Count;
            }
            else
            {
                // Если товара нет в корзине, добавляем новый
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                {
                    return NotFound("Товар не найден.");
                }

                CartElement NewCartElement = new()
                {
                    CartElementId = Guid.NewGuid(),
                    Product = product,
                    Count = request.Count
                };

                cart.CartElement.Add(NewCartElement);
                _context.CartElements.Add(NewCartElement);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ShowCart()
        {
            // Получаем UserId из куки
            if (!HttpContext.Request.Cookies.TryGetValue("UserId", out var user))
            {
                return NotFound("User id не найден");
            }

            Guid userId = new Guid(user);

            // Находим корзину пользователя с загрузкой связанных данных
            var cart = await _context.Carts
                .Include(c => c.CartElement)
                .ThenInclude(ce => ce.Product)
                .FirstOrDefaultAsync(c => c.CartId == userId);

            if (cart == null)
            {
                return Ok(new { message = "Корзина пуста." });
            }

            // Формируем список продуктов и их цен
            var products = cart.CartElement
                .Select(ce => new
                {
                    Name = ce.Product.Name,
                    Price = ce.Product.Price,
                    Count = ce.Count
                })
                .ToList();

            // Возвращаем данные в формате JSON
            return Ok(new
            {
                CartId = userId,
                Products = products
            });
        }

        public class AddToCartRequest
        {
            public Guid ProductId { get; set; }
            public int Count { get; set; }
        }

        private Guid GetCurrentUserId()
        {
            // Пример для ASP.NET Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                return result;
            }
            throw new UnauthorizedAccessException("User is not authenticated");
        }
    }
}
