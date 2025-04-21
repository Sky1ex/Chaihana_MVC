using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Exceptions;
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
            try
            {
                var userId = await _userService.AutoLogin();
                var cart = await _cartService.GetCartAsync(userId);
                var addresses = await _cartService.GetUserAddressesAsync(userId); // Получение адресов пользователя
                ViewBag.Addresses = addresses;
                return PartialView("_CartContentPartial", cart);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex)
                });
            }
        }

        [HttpGet("Api/Cart/ShowCart")]
        public async Task<IActionResult> GetCartElement()
        {
            try
            {
                var userId = await _userService.AutoLogin();
                var cart = await _cartService.GetCartAsync(userId);
                var addresses = await _cartService.GetUserAddressesAsync(userId); // Получение адресов пользователя
                return Ok(cart.Products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex),
                    Details = "Корзина не найдена"
                });
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

        [HttpPost("Api/Cart/UpdateCartItemCount")]
        public async Task<IActionResult> UpdateCartItemQuantity(Guid productId, int change)
        {
            try
            {
				var userId = await _userService.AutoLogin();
				await _cartService.UpdateCartItemQuantityAsync(userId, productId, change);
				return Ok();
			}
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex),
                    Details = "Корзина не найдена"
                });
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

		[HttpPost("Api/Cart/CheckoutSelected")]
        public async Task<IActionResult> CheckoutSelected([FromBody] CheckoutSelectedDto request)
        {
            try
            {
                var userId = await _userService.AutoLogin();
                var order = await _cartService.CheckoutSelectedAsync(userId, request.ProductIds, request.AddressId);
                return Ok(order);
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


        // Доделать страницу оформления заказа! Идея такова: при помощи кнопки оформления заказа в корзине переходим в нужную страницу с параметрами(закомментированы) через ajax или razor. Сохранять промежуточные данные в бд не будем.
        // Изменения: PaymentPageDto, Views/Cart/Index. Для отмены, использовать код выше.
        // Планы: добавить репозитории или mock-тесты. Думаю, для начала написать mock-тесты на сервисы. Затем добавить репозитории и изменить DTO. Далее дописать mock-тесы. Дальше code-review. Еще можно изменить авторизация через jwt.

		/*[HttpGet("Cart/CheckoutSelected")]
		public async Task<IActionResult> CheckoutSelected(*//*[FromBody] CheckoutSelectedDto request*//*)
		{
            CheckoutSelectedDto request = new CheckoutSelectedDto();

            return View("Index", request);
		}*/

		[HttpPost("Api/Cart/Purshare")]
        public async Task<IActionResult> Checkout(Guid addressId)
        {
            try
            {
                var userId = await _userService.AutoLogin();
                var order = await _cartService.CheckoutAsync(userId, addressId);
                return Ok(order);
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
