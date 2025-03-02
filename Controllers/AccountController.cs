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

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("autologin")]
        public async Task<IActionResult> AutoLogin()
        {
            var userId = await _userService.AutoLogin();
            return Ok($"User ID: {userId}");
        }
    }
}
