using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SuppliersController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpGet("{id}")]
		public async Task<Supplier?> GetSupplierAsync(long id)
		{
			Supplier supplier = await context.Suppliers.Include(s => s.Products).FirstAsync(s => s.SupplierId == id);
			supplier.Products?.ToList().ForEach(p => p.Supplier = null);
			return supplier;
		}

		[HttpPatch("{id}")]
		public async Task<Supplier?> PatchSupplier(long id, JsonPatchDocument<Supplier> patchDocument)
		{
			Supplier? supplier = await context.Suppliers.FindAsync(id);
			if (supplier != null)
			{
				patchDocument.ApplyTo(supplier);
				await context.SaveChangesAsync();
			}
			return supplier;
		}
	}
}