using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.OtherClasses;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class MenuController : Controller
    {

        private readonly ICartService _cartService;
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public MenuController(ICartService cartService, UserService userService, ApplicationDbContext context)
        {
            _context = context;
            _userService = userService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем список продуктов
            var products = await _context.Products.ToListAsync();

            // Получаем данные корзины
            /*var userId = await _userService.AutoLogin();
            var cart = await _cartService.GetCartAsync(userId);*/

            return View(products);
        }
    }
}
