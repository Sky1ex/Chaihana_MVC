using WebApplication1.DataBase;
using WebApplication1.Models;

namespace WebApplication1.OtherClasses
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> AutoLogin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            // Проверяем, есть ли идентификатор в куках
            if (!httpContext.Request.Cookies.TryGetValue("UserId", out var userIdString))
            {
                // Генерируем новый GUID
                var userId = Guid.NewGuid();

                // Сохраняем GUID в куки
                httpContext.Response.Cookies.Append("UserId", userId.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddYears(1), // Куки будут храниться год
                    HttpOnly = true, // Защита от XSS
                    Secure = true // Только для HTTPS
                });

                // Создаем нового пользователя
                var user = new User { UserId = userId };
                /*var cart = new Cart(user)*//* { User = user }*//*;*/
                var cart = new Cart()
                {
                    CartId = Guid.NewGuid(),
                    User = user
                };

                // Сохраняем в базу данных
                _dbContext.Users.Add(user);
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();

                return userId;
            }
            else
            {
                // Если кука уже существует, возвращаем существующий GUID
                return Guid.Parse(userIdString);
            }
        }
    }
}
