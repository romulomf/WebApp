using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Pages
{
	public class HandlerSelectorModel(DataContext context) : PageModel
	{
		private readonly DataContext context = context;

		public Product? Product { get; set; }

		public async Task OnGetAsync(long id = 1) => Product = await context.Products.FindAsync(id);

		public async Task OnGetRelatedAsync(long id = 1)
		{
			Product = await context.Products.Include(p => p.Supplier).Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
			if (Product != null && Product.Supplier != null)
			{
				Product.Supplier.Products = null;
			}
			if (Product != null && Product.Category != null)
			{
				Product.Category.Products = null;
			}
		}
	}
}