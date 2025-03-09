using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.DataBase;
using WebApplication1.Models;
using WebApplication1.OtherClasses;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserService _userService;

        public CartController(ICartService cartService, UserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        [HttpPost("Cart/AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            var userId = await _userService.AutoLogin();
            await _cartService.AddToCartAsync(userId, request.ProductId, request.Count);
            return Ok();
        }

        [HttpGet("Cart/ShowCart")]
        public async Task<IActionResult> GetCart()
        {
            var userId = await _userService.AutoLogin();
            var cart = await _cartService.GetCartAsync(userId);
            return PartialView("_CartContentPartial", cart);/*return Ok(cart);*/
        }

        [HttpPost("Cart/Purshare")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto request)
        {
            var userId = await _userService.AutoLogin();
            var order = await _cartService.CheckoutAsync(userId, request.AddressId);
            return Ok(order);
        }
    }
}

public class PurshareDto
{
    public string AddressId { get; set; }
}

public class AddToCartDto
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
}

public class CheckoutDto
{
    public Guid AddressId { get; set; }
}