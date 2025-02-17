using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataBase;

namespace WebApplication1.Controllers
{
    public class ProductCountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCountController(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}
