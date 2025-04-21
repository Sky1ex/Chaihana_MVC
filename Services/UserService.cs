using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Repository.Default;

namespace WebApplication1.OtherClasses
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> GetUser(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            return new UserDto
            {
                userId = user.UserId,
                phone = user.Phone,
                name = user.Name
            };
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
				var cart = new Cart() { CartId = Guid.NewGuid() };

				// Создаем нового пользователя
				var user = new User { UserId = userId, Cart = cart };
				cart.User = user;

				// Сохраняем в базу данных
				await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();

                return userId;
            }
            else
            {
                // Если кука уже существует, возвращаем существующий GUID
                return Guid.Parse(userIdString);
            }
        }

        public bool SetLogin(Guid login)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            // Сохраняем GUID в куки
            httpContext.Response.Cookies.Append("UserId", login.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1), // Куки будут храниться год
                HttpOnly = true, // Защита от XSS
                Secure = true // Только для HTTPS
            });

            return true;
        }
    }
}
