using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using System;
using WebApplication1.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApplication1.DataBase_and_more;

namespace WebApplication1.Controllers
{
    public class AutoLoginMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AutoLoginMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(
        HttpContext context,
        ApplicationDbContext dbContext,
        IConfiguration config)
        {
            // Проверяем куку
            var jwtToken = context.Request.Cookies["jwt_token"];
            Guid userId;

            if (string.IsNullOrEmpty(jwtToken) || !AuthOptions.ValidateToken(jwtToken, out userId))
            {
                // Создаем нового пользователя
                userId = Guid.NewGuid();
                var cart = new Cart { CartId = Guid.NewGuid() };
                var user = new User { UserId = userId, Cart = cart };

                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();

                // Устанавливаем новую куку с токеном
                AuthOptions.SetJwtCookie(context, userId);

                // Добавляем токен в заголовки текущего запроса
                context.Request.Headers.Append("Authorization", $"Bearer {AuthOptions.GenerateJwtToken(userId)}");
            }
            else
            {
                // Если токен валиден, добавляем его в заголовки
                context.Request.Headers.Append("Authorization", $"Bearer {jwtToken}");
            }

            await _next(context);
        }

    }
}
