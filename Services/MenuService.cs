using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.Repository.Default;

namespace WebApplication1.Services
{
    public class MenuService
    {
        private readonly ILogger<CartService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(ILogger<CartService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Category>> GetCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.ToList();
        }
    }
}
