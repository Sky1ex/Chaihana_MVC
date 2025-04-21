using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Exceptions;
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
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex)
                });
            }
        }

        [HttpGet("Api/Booking/GetAll")]
        public async Task<IActionResult> GetAllBookings(int tableId)
        {
            try
            {
                var booking = await _bookingService.GetAllBookingsByTableId(tableId);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex),
                    Details = ex is ValidationException ? null : ex.Message
                });
            }
        }

        [HttpPost("Api/Booking/Add")]
        public async Task<IActionResult> AddBooking(int tableId, DateTime time)
        {
            try
            {
                var userId = await _userService.AutoLogin();
                await _bookingService.AddBooking(tableId, time, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex),
                    Details = ex is ValidationException ? null : ex.Message
                });
            }
        }
    }
}
