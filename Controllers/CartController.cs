using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.Sessions;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return Json(new { succes = false, message = "Товар не найден" });
            }

            var cart = HttpContext.Session.Get<List<Product>>("Cart") ?? new List<Product>();
            cart.Add(product);
            HttpContext.Session.Set("Cart", cart);

            return Json(new { succes = true, message = "Товар добавлен в корзину!" } );
        }

        public async Task<IActionResult> CartShow()
        {
            var products = await _context.ProductsCount.ToListAsync();
            return View(products);
        }
    }
}
