using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
	public class HomeController(DataContext context) : Controller
	{
		private readonly DataContext context = context;

		public async Task<IActionResult> Index(long id = 1)
		{
			Product? product = await context.Products.FindAsync(id);
			if (product?.CategoryId == 1)
			{
				return View("Watersports", product);
			}
			else
			{
				return View(product);
			}
		}

		public IActionResult Common() => View();

		public IActionResult List() => View(context.Products);
	}
}