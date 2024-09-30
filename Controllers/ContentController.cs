using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ContentController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpGet("string")]
		[OutputCache(PolicyName = "30sec")]
		[Produces("application/json")]
		public string GetString() => $"{DateTime.Now.ToLongTimeString()} string response";

		[HttpGet("object/{format?}")]
		[DisableRateLimiting]
		[FormatFilter]
		[Produces("application/json", "application/xml")]
		public async Task<ProductBindingTarget> GetProductAsync()
		{
			Product product = await context.Products.FirstAsync();
			return new() { CategoryId = product.CategoryId, Name = product.Name, Price = product.Price, SupplierId = product.SupplierId };
		}

		[HttpPost]
		[Consumes("application/json")]
		public string SaveProductJson(ProductBindingTarget product)
		{
			return $"JSON: {product.Name}";
		}
		
		//[HttpPost]
		//[Consumes("application/xml")]
		//public string SaveProductXml(ProductBindingTarget product)
		//{
		//	return $"XML: {product.Name}";
		//}
	}
}