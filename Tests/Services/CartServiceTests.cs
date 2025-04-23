using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Mapster;
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

		Guid productId1 = new Guid("00000000-0000-0000-0000-000000000001"), productId2 = new Guid("00000000-0000-0000-0000-000000000002");
		Product product1, product2; ProductDto productDto1, productDto2;
        
        Guid cartElementId1 = new Guid("00000000-0000-0000-0000-000000000003"), cartElementId2 = new Guid("00000000-0000-0000-0000-000000000004");
        CartElement cartElement1, cartElement2; CartElementDto cartElementDto1, cartElementDto2;

        Guid cartId = new Guid("00000000-0000-0000-0000-000000000005");
        Cart cart; CartDto cartDto;

        Guid userId = new Guid("00000000-0000-0000-0000-000000000006");
        User user; UserDto userDto;

        Guid addressId = new Guid("00000000-0000-0000-0000-000000000008");
        Adress address; AddressDto addressDto;


		private readonly ApplicationDbContext _context;
		private readonly IUnitOfWork _unitOfWork;
        private IMapper _mappingCondig;
		private readonly CartService _cartService;

		public CartServiceTests() 
        {
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: "Test_Db")
			.Options;

			_context = new ApplicationDbContext(options);
			_unitOfWork = new UnitOfWork(_context);
            var config = new TypeAdapterConfig();
            _mappingCondig = new Mapper(config);
			_cartService = new CartService(Mock.Of<ILogger<CartService>>(), _unitOfWork, _mappingCondig);

            new MappingConfig().Register(config);

            // Заполняем тестовыми данными
            SeedTestData();
		}

        private async void SeedTestData()
        {
			category = new Category { Name = "Test", CategoryId = new Guid("00000000-0000-0000-0000-000000000007") };

			product1 = new Product { ProductId = productId1, ImageUrl = "Images\\img1", Name = "Кофе", Price = 200, Weight = 150, Category = category };
            productDto1 = _mappingCondig.Map<ProductDto>(product1);
            cartElement1 = new CartElement { Product = product1, Count = 2, CartElementId = cartElementId1 };
			product2 = new Product { ProductId = productId2, ImageUrl = "Images\\img2", Name = "Чай", Price = 175, Weight = 125, Category = category };
            productDto2 = _mappingCondig.Map<ProductDto>(product2);
            cartElement2 = new CartElement { Product = product2, Count = 3, CartElementId = cartElementId2 };

            cart = new Cart
			{
				CartId = cartId,
				CartElement = new List<CartElement>
				{
                    cartElement1,
                    cartElement2
                }
			};

            user = new User { Name = "Test", Cart = cart, UserId = userId };
            userDto = new UserDto { name = "Test", userId = userId };
            cart.User = user;

            address = new Adress
            {
                AdressId = addressId,
                City = "city",
                Street = "street",
                House = "house",
                Apartment = 1
            };

            addressDto = _mappingCondig.Map<AddressDto>(address);

            _context.Categories.Add(category);
			_context.Carts.Add(cart);
			_context.Users.Add(user);
			_context.CartElements.Add(cartElement1);
            _context.CartElements.Add(cartElement2);
            _context.Products.Add(product1);
			_context.Products.Add(product2);
            _context.Adresses.Add(address);
            await _context.SaveChangesAsync();

        }

		[Fact]
        public async Task GetCartAsyncTest()
        {
			var result = await _cartService.GetCartAsync(userId);

            cartDto = _mappingCondig.Map<CartDto>(cart);

            result.Should().NotBeNull();

            result.Should().BeEquivalentTo(cartDto, options =>
            {
                options.ComparingByMembers<CartDto>();
                options.ComparingByMembers<CartElementDto>();
                options.ComparingByMembers<ProductDto>();
                return options;
            });
        }

        [Fact]
        public async Task RemoveFromCartAsyncTest()
        {

            cartDto = _mappingCondig.Map<CartDto>(cart);
            cartDto.CartElement.RemoveAt(0);

            await _cartService.RemoveFromCartAsync(userId, productId1);

            var result = await _cartService.GetCartAsync(userId);

            result.Should().NotBeNull();
            result.CartElement.Should().HaveCount(1);
            _context.CartElements.Should().HaveCount(1);
            result.Should().BeEquivalentTo(cartDto, options =>
            {
                options.ComparingByMembers<CartDto>();
                options.ComparingByMembers<CartElementDto>();
                options.ComparingByMembers<ProductDto>();
                return options;
            });
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsyncTest()
        {
            cartDto = _mappingCondig.Map<CartDto>(cart);
            cartDto.CartElement.ElementAt(0).Count = 3;

            await _cartService.UpdateCartItemQuantityAsync(userId, productId1, 1);

            var result = await _cartService.GetCartAsync(userId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(cartDto, options =>
            {
                options.ComparingByMembers<CartDto>();
                options.ComparingByMembers<CartElementDto>();
                options.ComparingByMembers<ProductDto>();
                return options;
            });
        }

        // Доделать!
        [Fact]
        public async Task CheckoutSelectedAsyncTest()
        {

            await _cartService.CheckoutSelectedAsync(userId, new List<Guid> { productId1, productId2 }, addressId);

            OrderDto orderDtoResult = _mappingCondig.Map<OrderDto>(_context.Orders.First());

            OrderDto orderDtoCurrent = new OrderDto
            {
                Address = addressDto,
                Products = new List<OrderElementDto>
                {
                    new OrderElementDto
                    {
                        Count = 2,
                        Product = productDto1,
                        ProductId = productId1,
                    },
                    new OrderElementDto
                    {
                        Count = 3,
                        Product = productDto2,
                        ProductId = productId2,
                    }
                },
                OrderId = orderDtoResult.OrderId,
                DateTime = orderDtoResult.DateTime,
            };

            var resultCart = await _cartService.GetCartAsync(userId);

            resultCart.CartElement.Should().BeEmpty();

            orderDtoResult.Should().BeEquivalentTo(orderDtoCurrent, options =>
            {
                options.ComparingByMembers<AddressDto>();
                options.ComparingByMembers<OrderElementDto>();
                options.ComparingByMembers<ProductDto>();
                return options;
            });
        }
    }
}
