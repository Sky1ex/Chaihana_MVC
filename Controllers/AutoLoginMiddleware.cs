using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using System;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AutoLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Проверяем, есть ли идентификатор в куках
            if (!context.Request.Cookies.TryGetValue("UserId", out var userIdCookie) || !Guid.TryParse(userIdCookie, out var userId))
            {
                // Генерируем новый GUID
                userId = Guid.NewGuid();

                // Сохраняем GUID в куки
                context.Response.Cookies.Append("UserId", userId.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddYears(1), // Куки будут храниться год
                    HttpOnly = true, // Защита от XSS
                    Secure = true // Только для HTTPS
                });

				var cart = new Cart() { CartId = Guid.NewGuid() };

				// Создаем нового пользователя
				var user = new User { UserId = userId, Cart = cart };
                cart.User = user;

                // Сохраняем в базу данных
                dbContext.Users.Add(user);
                dbContext.Carts.Add(cart);
                await dbContext.SaveChangesAsync();
            }

            // Передаем запрос дальше по конвейеру middleware
            await _next(context);
        }
    }

}
