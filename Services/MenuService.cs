using WebApplication1.DataBase;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class MenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public MenuService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Category>> GetCategories()
        {
            var categories = _context.Categories.ToList();
            return categories;
        }
    }
}
