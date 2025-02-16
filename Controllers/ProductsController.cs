using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        /*public IActionResult Index1()
        {
            return View(); // Возвращает представление Index.cshtml
        }

        public IActionResult Details(int id)
        {
            ViewBag.ProductId = id; // Передаем id в представление
            return View(); // Возвращает представление Details.cshtml
        }*/

        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index1()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
    }
}
