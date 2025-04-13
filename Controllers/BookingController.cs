using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataBase;
using WebApplication1.OtherClasses;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class BookingController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly BookingService _bookingService;

        public BookingController(UserService userService, ApplicationDbContext context, BookingService bookingService)
        {
            _userService = userService;
            _context = context;
            _bookingService = bookingService;
        }

        [HttpGet("Booking")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Api/Booking/Add")]
        public async Task<IActionResult> AddBooking(int tableId, DateTime time)
        {
            var userId = await _userService.AutoLogin();
            await _bookingService.AddBooking(tableId, time, userId);
            return Ok();
        }
    }
}
