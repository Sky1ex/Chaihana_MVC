using Mapster;
using WebApplication1.Models;

namespace WebApplication1.DTO
{
	public class MappingConfig : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			// Cart -> CartDto
			config.NewConfig<Cart, CartDto>()
				.Map(dest => dest.CartId, src => src.CartId)
				.Map(dest => dest.Products, src =>
					src.CartElement != null
						? src.CartElement.Adapt<List<CartProductDto>>()
						: new List<CartProductDto>());

			// CartElement -> CartProductDto
			config.NewConfig<CartElement, CartProductDto>()
				.Map(dest => dest.ProductId, src => src.Product.ProductId)
				.Map(dest => dest.Name, src => src.Product.Name)
				.Map(dest => dest.Price, src => src.Product.Price)
				.Map(dest => dest.ImageUrl, src => src.Product.ImageUrl)
				.Map(dest => dest.Count, src => src.Count);

			// Product -> CartProductDto (опционально, если нужно прямое преобразование)
			config.NewConfig<Product, CartProductDto>()
				.Map(dest => dest.ProductId, src => src.ProductId)
				.Map(dest => dest.Name, src => src.Name)
				.Map(dest => dest.Price, src => src.Price)
				.Map(dest => dest.ImageUrl, src => src.ImageUrl)
				.Map(dest => dest.Count, src => 1); // Значение по умолчанию
		}
	}
}
