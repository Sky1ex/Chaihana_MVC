using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DataBase;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Net;
using WebApplication1.OtherClasses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApplication1.DTO;
using WebApplication1.Services;
using Twilio.Rest.Trunking.V1;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

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
        public IActionResult Addresses()
        {
            var userId = _userService.AutoLogin().Result;
            var addresses = _accountService.GetAddresses(userId);

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

            var user = await _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId.Result);

            var address = new Adress { AdressId = Guid.NewGuid(), City = request.City, House = request.House, Street = request.Street };

            user.Adresses.Add(address);
            _context.Adresses.Add(address);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Account/DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(string addressId)
        {
            var userId = _userService.AutoLogin();

            var user = _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefault(c => c.UserId == userId.Result);

            var address = user.Adresses.FirstOrDefault(c => c.AdressId == new Guid(addressId));


            user.Adresses.Remove(address);
            _context.Adresses.Remove(address);
            await _context.SaveChangesAsync();

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
        public async Task<IActionResult> CheckCode(string code)
        {
            var userId = _userService.AutoLogin().Result;

            bool flag = await _accountService.CheckCode(code, userId);

            if (flag)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Account/AddName")]
        public async Task<IActionResult> AddName(string name)
        {
            var userId = _userService.AutoLogin().Result;

            await _accountService.AddName(name, userId);

            return Ok();
        }
    }

    public class PhoneRequest
    {
        public string number { get; set; }
        public string destination { get; set; }
        public string text { get; set; }
    }
}

// Доделать представления для Orders, Adresses и UserData(смотри Account/Index)