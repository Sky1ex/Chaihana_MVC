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
        public async Task<IActionResult> UpdateCartItemQuantity(Guid productId, int change)
        {
            var userId = await _userService.AutoLogin();
            await _cartService.UpdateCartItemQuantityAsync(userId, productId, change);
            return Ok();
        }

		/*[HttpPost("Cart/CheckoutSelected")]
        public async Task<IActionResult> CheckoutSelected([FromBody] CheckoutSelectedDto request)
        {
            var userId = await _userService.AutoLogin();
            var order = await _cartService.CheckoutSelectedAsync(userId, request.ProductIds, request.AddressId);
            return Ok(order);
        }*/


        // Доделать страницу оформления заказа! Идея такова: при помощи кнопки оформления заказа в корзине переходим в нужную страницу с параметрами(закомментированы) через ajax или razor. Сохранять промежуточные данные в бд не будем.
        // Изменения: PaymentPageDto, Views/Cart/Index. Для отмены, использовать код выше.
        // Планы: добавить репозитории или mock-тесты. Думаю, для начала написать mock-тесты на сервисы. Затем добавить репозитории и изменить DTO. Далее дописать mock-тесы. Дальше code-review.

		[HttpGet("Cart/CheckoutSelected")]
		public async Task<IActionResult> CheckoutSelected(/*[FromBody] CheckoutSelectedDto request*/)
		{
            CheckoutSelectedDto request = new CheckoutSelectedDto();

            return View("Index", request);
		}

		[HttpPost("Cart/Purshare")]
        public async Task<IActionResult> Checkout(Guid addressId)
        {
            var userId = await _userService.AutoLogin();
            var order = await _cartService.CheckoutAsync(userId, addressId);
            return Ok(order);
        }
    }
}
