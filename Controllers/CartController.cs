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
        private readonly AccountService _accountService;

        public CartController(ICartService cartService, UserService userService, AccountService accountService)
        {
            _cartService = cartService;
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet("Cart/ShowCart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = await _userService.GetLogin();
                var cart = await _cartService.GetCartAsync(userId);
                var addresses = await _accountService.GetAddresses(userId); // Получение адресов пользователя
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
                var userId = await _userService.GetLogin();
                var cart = await _cartService.GetCartAsync(userId);
                var addresses = await _accountService.GetAddresses(userId); // Получение адресов пользователя
                return Ok(cart.CartElement);
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
				var userId = await _userService.GetLogin();
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

        [HttpGet("Cart/CheckoutSelected")]
        public async Task<IActionResult> CheckoutSelected([FromQuery] List<string> products)
        {
            try
            {
                List<Guid> productIds = products.Select(x => new Guid(x)).ToList();
                var userId = await _userService.GetLogin();
                var cart = await _cartService.GetCartAsync(userId);
                List<CartElementDto> items = cart.CartElement.Where(x => productIds.Contains(x.ProductId)).ToList();
                var addresses = await _accountService.GetAddresses(userId);
                ViewBag.Address = addresses.FirstOrDefault();
                return View("Index", items);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Message = ErrorViewModel.GetUserFriendlyMessage(ex)
                });
            }
        }

        // В БД и модель для корзины добавить сущность сумму(расчет будет в бд). Для готового заказа добавить сущность (способ оплаты - Payment). Также добавить появление карты при кнопке изменить.
        // На самой карте добавить уже существующие адресы пользователя, которые можно выбрать. На карте будут 2 кнопки: добавить и выбрать. При выборе адрес не сохраняется. Доделать тесты!.

        /*[HttpPost("Api/Cart/Purshare")]
        public async Task<IActionResult> Purshare(List<Guid> orderElements, Guid addressId)
        {
            try
            {
                var userId = await _userService.GetLogin();
                var order = await _cartService.CheckoutSelectedAsync(userId, orderElements, addressId);
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
        }*/

        [HttpPost("Api/Cart/Purshare")]
        public async Task<IActionResult> Purshare(List<Guid> orderElements, Guid addressId)
        {
            try
            {
                var userId = await _userService.GetLogin();
                if (await _userService.CheckPhone(userId))
                {
                    var order = await _cartService.CheckoutSelectedAsync(userId, orderElements, addressId);
                    return Ok(order);
                }
                else
                {
                    return BadRequest("Для заказа необходимо зарегистрироваться!");
                }
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
