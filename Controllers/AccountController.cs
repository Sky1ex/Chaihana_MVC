using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataBase;
using Microsoft.EntityFrameworkCore;
using WebApplication1.OtherClasses;
using WebApplication1.DTO;
using WebApplication1.Services;
using WebApplication1.Exceptions;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        private readonly BookingService _bookingService;

        public AccountController(UserService userService, AccountService accountService, BookingService bookingService)
        {
            _userService = userService;
            _accountService = accountService;
            _bookingService = bookingService;
        }

        [HttpGet("Api/Login")]
        public async Task<IActionResult> AutoLogin()
        {
            try
            {
                var userId = await _userService.GetLogin();
                return Ok($"User ID: {userId}");
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

        [HttpGet("Account")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = await _userService.GetLogin();
                var addresses = await _accountService.GetAddresses(userId);
                var orders = await _accountService.GetOrders(userId);
                var bookings = await _bookingService.GetAllBookingsByUserId(userId);
                var user = await _userService.GetUser(userId);
                ViewBag.Orders = orders;
                ViewBag.Addresses = addresses;
                ViewBag.Bookings = bookings;
                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex)
                });
            }
        }

        [HttpPost("Api/Account/AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto request)
        {
            try
            {
                var userId = await _userService.GetLogin();

                await _accountService.AddAddress(request.City, request.Street, request.House, request.Apartment, userId);

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

        [HttpPut("Api/Account/PutAddress")]
		public async Task<IActionResult> PutAddress([FromBody] AddressDto request)
		{
            try
            {
                var userId = await _userService.GetLogin();

                await _accountService.PutAddress(request.City, request.Street, request.House, request.Apartment, userId, request.AddressId);

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

		[HttpDelete("Api/Account/DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(string addressId)
        {
            try
            {
                var userId = await _userService.GetLogin();

                await _accountService.DeleteAddress(addressId, userId);

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

        [HttpGet("Api/Account/GetCode")]
        public async Task<IActionResult> GetCode(string userNumber)
        {
            try
            {
                var userId = _userService.GetLogin().Result;
                using var response = await _accountService.PostSms(userNumber, userId);

                return StatusCode((int)response.StatusCode);
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

        [HttpPost("Api/Account/CheckCode")]
        public async Task<IActionResult> CheckCode(string code)
        {
            try
            {
                var userId = await _userService.GetLogin();
                string answer = await _accountService.CheckCode(code, userId);
                if (!(answer == "true" || answer == "false")) _userService.SetLogin(Guid.Parse(answer));
                return Ok(answer);
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

        [HttpPost("Api/Account/AddName")]
        public async Task<IActionResult> AddName(string name)
        {
            try
            {
                var userId = _userService.GetLogin().Result;
                await _accountService.AddName(name, userId);

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

        [HttpGet("Api/Account/GetAddresses")]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userId = _userService.GetLogin().Result;

                var addresses = await _accountService.GetAddresses(userId);

                return Ok(addresses);
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

// Доделать представления для Orders, Adresses и UserData(смотри Account/Index)