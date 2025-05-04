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

		[HttpGet("Api/Booking/GetByDate")]
		public async Task<IActionResult> GetAllBookingsByDate(int tableId, DateTime time)
		{
			try
			{
				var booking = await _bookingService.GetAllBookingsByTableIdAndDate(tableId, time);
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

		// Доделать обработку всех ошибок в js!!!

		[HttpPost("Api/Booking/Add")]
        public async Task<IActionResult> AddBooking(int tableId, DateTime time, int interval)
        {
            try
            {
                var userId = await _userService.GetLogin();
                if(await _bookingService.AddBooking(tableId, time, interval, userId)) return Ok();
                else return BadRequest("Время уже занято или интервал превышает 3 часа!");
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


        [HttpDelete("Api/Booking/Delete")]
        public async Task<IActionResult> DeleteBooking(Guid bookingId)
        {
            try
            {
                await _bookingService.DeleteBooking(bookingId);
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
