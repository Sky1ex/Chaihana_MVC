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

        private readonly MenuService _menuService;
        private readonly ApplicationDbContext _context;

        public MenuController(MenuService menuService, ApplicationDbContext context)
        {
            _context = context;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем список продуктов
            var products = await _context.Products.ToListAsync();

            ViewBag.Categories = await _menuService.GetCategories();

            return View(products);
        }
    }
}
