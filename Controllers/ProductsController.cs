using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index1()
        {
            // Проблема из за конструктора Cart
            var products = _context.Products.ToList(); // Получаем список товаров из базы данных
            return View(products); // Возвращает представление Index.cshtml
        }

        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
