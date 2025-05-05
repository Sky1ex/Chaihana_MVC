using Mapster;
using WebApplication1.Models;

namespace WebApplication1.DTO
{
	public class MappingConfig : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{

            // CartElement -> CartElementDto
            config.NewConfig<CartElement, CartElementDto>()
                .Map(dest => dest.ProductId, src => src.Product.ProductId)
                .Map(dest => dest.product, src => src.Product)
                .Map(dest => dest.Count, src => src.Count);

            // Product -> ProductDto
            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Price, src => src.Price)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            // Cart -> CartDto
            config.NewConfig<Cart, CartDto>()
				.Map(dest => dest.CartId, src => src.CartId)
                .Map(dest => dest.CartElement, src => src.CartElement);

            // Adress -> AddressDto
            config.NewConfig<Adress, AddressDto>()
				.Map(dest => dest.AddressId, src => src.AdressId)
				.Map(dest => dest.City, src => src.City)
				.Map(dest => dest.Street, src => src.Street)
				.Map(dest => dest.Apartment, src => src.Apartment)
                .Map(dest => dest.House, src => src.House);

            // OrderElement -> OrderElementDto
            config.NewConfig<OrderElement, OrderElementDto>()
				.Map(dest => dest.ProductId, src => src.OrderElementId)
				.Map(dest => dest.Product, src => src.Product)
				.Map(dest => dest.Count, src => src.Count);

            // Booking -> BookingDto
            config.NewConfig<Booking, BookingDto>()
				.Map(dest => dest.BookingId, src => src.BookingId)
				.Map(dest => dest.Time, src => src.Time)
				.Map(dest => dest.Interval, src => src.Interval)
				.Map(dest => dest.Table, src => src.Table);

            // User -> UserDto
            config.NewConfig<User, UserDto>()
				.Map(dest => dest.userId, src => src.UserId)
				.Map(dest => dest.name, src => src.Name)
				.Map(dest => dest.phone, src => src.Phone);

			// User -> UserDto
			config.NewConfig<Order, OrderDto>()
				.Map(dest => dest.OrderId, src => src.OrderId)
				.Map(dest => dest.DateTime, src => src.dateTime)
				.Map(dest => dest.Address, src => src.Adress)
				.Map(dest => dest.Products, src => src.OrderElement);
                
        }
	}
}
