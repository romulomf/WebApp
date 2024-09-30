using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class ProductBindingTarget
	{
		[Required]
		public required string Name { get; set; }

		[Range(1, 1000)]
		public required decimal Price { get; set; }

		[Range(1, long.MaxValue)]
		public required long CategoryId { get; set; }

		[Range(1, long.MaxValue)]
		public required long SupplierId { get; set; }

		public Product ToProduct() => new() { Name = this.Name, Price = this.Price, CategoryId = this.CategoryId, SupplierId = this.SupplierId };
	}
}