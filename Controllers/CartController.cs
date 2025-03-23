using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.DataBase;
using WebApplication1.DTO;
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

        [HttpGet("Cart/ShowCart")]
        public async Task<IActionResult> GetCart()
        {
            var userId = await _userService.AutoLogin();
            var cart = await _cartService.GetCartAsync(userId);
            var addresses = await _cartService.GetUserAddressesAsync(userId); // Получение адресов пользователя
            ViewBag.Addresses = addresses;
            return PartialView("_CartContentPartial", cart);
        }

        [HttpGet("Api/Cart/ShowCart")]
        public async Task<List<CartProductDto>> GetCartElement()
        {
            var userId = await _userService.AutoLogin();
            var cart = await _cartService.GetCartAsync(userId);
            var addresses = await _cartService.GetUserAddressesAsync(userId); // Получение адресов пользователя
            return cart.Products;
        }

        [HttpPost("Cart/UpdateCartItemCount")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityDto request)
        {
            var userId = await _userService.AutoLogin();
            await _cartService.UpdateCartItemQuantityAsync(userId, request.ProductId, request.Change);
            return Ok();
        }

        [HttpPost("Cart/CheckoutSelected")]
        public async Task<IActionResult> CheckoutSelected([FromBody] CheckoutSelectedDto request)
        {
            var userId = await _userService.AutoLogin();
            var order = await _cartService.CheckoutSelectedAsync(userId, request.ProductIds, request.AddressId);
            return Ok(order);
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

public class UpdateCartItemQuantityDto
{
    public Guid ProductId { get; set; }
    public int Change { get; set; } // 1 для увеличения, -1 для уменьшения
}

public class CheckoutSelectedDto
{
    public List<Guid> ProductIds { get; set; }
    public Guid AddressId { get; set; }
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