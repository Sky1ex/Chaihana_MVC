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
        public IActionResult Index()
        {
            return View();
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
            var userId = _userService.AutoLogin();

            await _accountService.AddAddress(request.City, request.Street, request.House, userId.Result);

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
            var responseContent = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, responseContent);
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
    //c528fe2d-bf43-4549-988c-52e28043c85d
    public class PhoneRequest
    {
        public string number { get; set; }
        public string destination { get; set; }
        public string text { get; set; }
    }
}

// Доделать представления для Orders, Adresses и UserData(смотри Account/Index)