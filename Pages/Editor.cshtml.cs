using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages
{
	public class EditorModel(DataContext context) : PageModel
	{
		private readonly DataContext context = context;

		public Product? Product { get; set; }

		public async Task<IActionResult> OnGetAsync(long id)
		{
			Product = await context.Products.FindAsync(id);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(long id, decimal price)
		{
			Product? p = await context.Products.FindAsync(id);
			if (p != null)
			{
				p.Price = price;
			}
			await context.SaveChangesAsync();
			// usar o redirecionamento para a própria página, força o recarregamento dos dados da página
			return RedirectToPage();
		}
	}
}