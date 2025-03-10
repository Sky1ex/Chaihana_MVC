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

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public AccountController(UserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
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
            var user = _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId).Result;
            List<Models.Adress> Adresses = user.Adresses;

            return View(Adresses);
        }

        [HttpGet("Account/Orders")]
        public IActionResult Orders()
        {
            var userId = _userService.AutoLogin();
            var user = _context.Users
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.OrderElement)
                .ThenInclude(ced => ced.Product)
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.Adress)
                .FirstOrDefaultAsync(c => c.UserId == userId.Result);
            var orders = user.Result.Orders.ToList();

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
    }
}

// Доделать представления для Orders, Adresses и UserData(смотри Account/Index)