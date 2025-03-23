using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataBase;
using WebApplication1.OtherClasses;

namespace WebApplication1.Controllers
{
    public class BookingController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public BookingController(UserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet("Booking")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
