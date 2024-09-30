using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApp.Models;

namespace WebApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[EnableRateLimiting("fixedWindow")]
	public class ProductsController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpGet]
		public IAsyncEnumerable<Product> GetProducts()
		{
			return context.Products.AsAsyncEnumerable();
		}

		[HttpGet("{id}")]
		[DisableRateLimiting]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
		{
			logger.LogInformation("GetProduct Action Invoked");
			Product? product = await context.Products.FindAsync(id);
			if (product != null)
			{
				return Ok(product);
			}
			return NotFound();
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SaveProduct(ProductBindingTarget target)
		{
			if (ModelState.IsValid)
			{
				Product product = target.ToProduct();
				await context.Products.AddAsync(product);
				await context.SaveChangesAsync();
				return CreatedAtAction(nameof(GetProducts), product); 
			}
			return BadRequest(ModelState);
		}

		[HttpPut]
		public async Task UpdateProduct(Product product)
		{
			context.Products.Update(product);
			await context.SaveChangesAsync();
		}

		[HttpDelete("{id}")]
		public async Task DeleteProduct(long id)
		{
			context.Products.Remove(new() { ProductId = id, Name = string.Empty });
			await context.SaveChangesAsync();
		}
	}
}