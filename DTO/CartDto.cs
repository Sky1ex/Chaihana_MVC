namespace WebApplication1.DTO 
{ 
	public class CartDto 
	{ 
		public Guid CartId { get; set; } 
		public List<CartElementDto>? CartElement { get; set; } 
	} 
}
