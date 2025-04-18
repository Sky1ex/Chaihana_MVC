using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataBase;
using Microsoft.EntityFrameworkCore;
using WebApplication1.OtherClasses;
using WebApplication1.DTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        private readonly ApplicationDbContext _context;

        public AccountController(UserService userService, AccountService accountService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
            _accountService = accountService;
        }

        [HttpGet("Api/Login")]
        public async Task<IActionResult> AutoLogin()
        {
            var userId = await _userService.AutoLogin();
            return Ok($"User ID: {userId}");
        }

        [HttpGet("Account")]
        public async Task<IActionResult> Index()
        {
            var userId = await _userService.AutoLogin();
            var addresses = await _accountService.GetAddresses(userId);
            var orders = _accountService.GetOrders(userId);
            var user = await _context.Users
                .FirstOrDefaultAsync(c => c.UserId == userId);
            ViewBag.Orders = orders;
            ViewBag.Addresses = addresses;
            return View(user);
        }

        [HttpGet("Account/Addresses")]
        public async Task<IActionResult> Addresses()
        {
            var userId = await _userService.AutoLogin();
            var addresses = await _accountService.GetAddresses(userId);

            return View(addresses);
        }

        [HttpGet("Account/Orders")]
        public IActionResult Orders()
        {
            var userId = _userService.AutoLogin().Result;
            var orders = _accountService.GetOrders(userId);

            return View(orders);
        }

        [HttpGet("Account/UserData")]
        public IActionResult UserData()
        {
            var userId = _userService.AutoLogin();
            var user = _context.Users
                .FirstOrDefaultAsync(c => c.UserId == userId.Result).Result;

            return View(user);
        }

        [HttpPost("Account/AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto request)
        {
            var userId = await _userService.AutoLogin();

            await _accountService.AddAddress(request.City, request.Street, request.House, request.Apartment, userId);

            return Ok();
        }

        [HttpPut("Account/PutAddress")]
		public async Task<IActionResult> PutAddress([FromBody] AddressDto request)
		{
			var userId = await _userService.AutoLogin();

			await _accountService.PutAddress(request.City, request.Street, request.House, request.Apartment, userId, request.AddressId);

			return Ok();
		}

		[HttpDelete("Account/DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(string addressId)
        {
            var userId = await _userService.AutoLogin();

            _accountService.DeleteAddress(addressId, userId);

            return Ok();
        }

        [HttpGet("Account/GetCode")]
        public async Task<IActionResult> GetCode(string userNumber)
        {
            var userId = _userService.AutoLogin().Result;
            using var response = await _accountService.PostSms(userNumber, userId);
            
            return StatusCode((int)response.StatusCode);
        }

        [HttpPost("Account/CheckCode")]
        public async Task<string> CheckCode(string code)
        {
            var userId = _userService.AutoLogin().Result;
            string answer = await _accountService.CheckCode(code, userId);
            if (!(answer == "true" || answer == "false")) _userService.SetLogin(Guid.Parse(answer));
            return answer;
        }

        [HttpPost("Account/AddName")]
        public async Task<IActionResult> AddName(string name)
        {
            var userId = _userService.AutoLogin().Result;
            await _accountService.AddName(name, userId);

            return Ok();
        }
    }
}

// Доделать представления для Orders, Adresses и UserData(смотри Account/Index)