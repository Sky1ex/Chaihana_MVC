using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
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

        [HttpGet("Api/Booking/GetAll")]
        public async Task<List<BookingDto>> GetAllBookings(int tableId)
        {
            var booking = await _bookingService.GetAllBookingsByTableId(tableId);
            return booking;
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
