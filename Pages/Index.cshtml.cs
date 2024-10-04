using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages
{
	public class IndexModel(DataContext context) : PageModel
	{
		private DataContext context = context;

		public Product? Product { get; set; }

		public async Task<IActionResult> OnGetAsync(long id = 1)
		{
			Product = await context.Products.FindAsync(id);
			if (Product == null)
			{
				// usa o redirecionamento para uma outra página razor, com isso muda a URL para a rota da nova página assim que for feito
				return RedirectToPage("NotFound");
			}
			return Page();
		}
	}
}