using Castle.Components.DictionaryAdapter;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text.Json;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.Repository.Default;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests.Services
{
    public class CartServiceTests
    {
        Category category;

		Guid productId1 = Guid.NewGuid(), productId2 = Guid.NewGuid();
		Product product1, product2; CartProductDto productDto1, productDto2;
        
        Guid cartId = Guid.NewGuid();
        Cart cart; CartDto cartDto;

        Guid userId = Guid.NewGuid();
        User user; UserDto userDto;
        Mock<IUnitOfWork> mockUnit;
        Mock<ILogger<CartService>> mockLogger;
        Mock<IMapper> mockMapping;
        CartService cartService;

		private readonly ApplicationDbContext _context;
		private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mappingCondig;
		private readonly CartService _cartService;

		public CartServiceTests() 
        {
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "Test_Db")
			.Options;

			_context = new ApplicationDbContext(options);
			_unitOfWork = new UnitOfWork(_context);
            _mappingCondig = new Mapper();
			_cartService = new CartService(Mock.Of<ILogger<CartService>>(), _unitOfWork/*, _mappingCondig*/);

			// Заполняем тестовыми данными
			SeedTestData();
		}

        private void SeedTestData()
        {
			category = new Category { Name = "Test" };

			product1 = new Product { ProductId = productId1, ImageUrl = "Images\\img1", Name = "Кофе", Price = 200, Weight = 150, Category = category };
            productDto1 = new CartProductDto { ProductId = productId1, ImageUrl = "Images\\img1", Name = "Кофе", Price = 200, Count = 2 };
			product2 = new Product { ProductId = productId2, ImageUrl = "Images\\img2", Name = "Чай", Price = 175, Weight = 125, Category = category };
			productDto2 = new CartProductDto { ProductId = productId2, ImageUrl = "Images\\img2", Name = "Чай", Price = 175, Count = 3 };

			cart = new Cart
			{
				CartId = cartId,
				CartElement = new List<CartElement>
				{
					new CartElement { Product = product1, Count = 2 },
					new CartElement { Product = product2, Count = 3 }
				}
			};

            cartDto = new CartDto
            {
                CartId = userId,
                Products = new List<CartProductDto>
                {
                    productDto1, productDto2
                }
            };

			user = new User { Name = "Test", Cart = cart, UserId = userId };
            userDto = new UserDto { name = "Test", userId = userId };
			cart.User = user;

            _context.Categories.Add(category);
			_context.Carts.Add(cart);
			_context.Users.Add(user);
			_context.Products.Add(product1);
			_context.Products.Add(product2);
            _context.SaveChanges();
		}

		[Fact]
        public async Task GetCartAsyncTest()
        {

			var userId = _context.Users.First().UserId;
			var result = await _cartService.GetCartAsync(userId);

            Assert.NotNull(result);

			/*var expectedJson = JsonSerializer.Serialize(cartDto);
			var actualJson = JsonSerializer.Serialize(result);

			Assert.Equal(expectedJson, actualJson);*/
			Assert.Equal(_mappingCondig.Map<CartDto>(cart), result);
		}

        /*[Fact]
        public async Task RemoveFromCartAsync()
        {
            // 1. Настраиваем InMemoryDatabase
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var userId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            // 2. Заполняем базу тестовыми данными
            using (var dbContext = new ApplicationDbContext(options))
            {
                var user = new User { Name = "Test", UserId = userId };

                var product1 = new Product { ProductId = productId1, ImageUrl = "Images\\img1", Name = "Кофе", Price = 200, Weight = 150 };
                var product2 = new Product { ProductId = productId2, ImageUrl = "Images\\img2", Name = "Чай", Price = 175, Weight = 125 };

                var cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    User = user,
                    CartElement = new List<CartElement>
                    {
                        new CartElement { Product = product1, Count = 2 },
                        new CartElement { Product = product1, Count = 3 }
                    }
                };

                dbContext.Carts.Add(cart);
                dbContext.Users.Add(user);
                dbContext.Products.Add(product1);
                dbContext.Products.Add(product2);
                await dbContext.SaveChangesAsync();
            }

            // 3. Создаём новый контекст для сервиса (чистый для теста)
            using (var dbContext = new ApplicationDbContext(options))
            {
                var mockLogger = new Mock<ILogger<CartService>>();
                var cartService = new CartService(dbContext, mockLogger.Object);

                // 4. Вызываем метод
                await cartService.RemoveFromCartAsync(userId, productId1);

                // 5. Проверяем результат
                Assert.NotNull(result);
                Assert.Equal(2, result.Products.Count); // Пример проверки
            }
        }*/

        /*[Fact]
        public async Task GetUserAddressesAsyncTest()
        {
            // 1. Настраиваем InMemoryDatabase
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var userId = Guid.NewGuid();

            // 2. Заполняем базу тестовыми данными
            using (var dbContext = new ApplicationDbContext(options))
            {
                var user = new User { Name = "Test", UserId = userId };

                var product1 = new Product { ProductId = Guid.NewGuid(), ImageUrl = "Images\\img1", Name = "Кофе", Price = 200, Weight = 150 };
                var product2 = new Product { ProductId = Guid.NewGuid(), ImageUrl = "Images\\img2", Name = "Чай", Price = 175, Weight = 125 };

                var cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    User = user,
                    CartElement = new List<CartElement>
                    {
                        new CartElement { Product = product1, Count = 2 },
                        new CartElement { Product = product1, Count = 3 }
                    }
                };

                dbContext.Carts.Add(cart);
                dbContext.Users.Add(user);
                dbContext.Products.Add(product1);
                dbContext.Products.Add(product2);
                await dbContext.SaveChangesAsync();
            }

            // 3. Создаём новый контекст для сервиса (чистый для теста)
            using (var dbContext = new ApplicationDbContext(options))
            {
                var mockLogger = new Mock<ILogger<CartService>>();
                var cartService = new CartService(dbContext, mockLogger.Object);

                // 4. Вызываем метод
                var result = await cartService.GetCartAsync(userId);

                // 5. Проверяем результат
                Assert.NotNull(result);
                Assert.Equal(2, result.Products.Count); // Пример проверки
            }
        }*/
    }
}
