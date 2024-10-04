using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages
{
	public class NotFoundModel(DataContext context) : PageModel
	{
		private readonly DataContext context = context;

		public IEnumerable<Product> Products { get; set; } = [];

		public void OnGetAsync(long id = 1) => Products = context.Products;
	}
}
