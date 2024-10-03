using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
	public class HomeController(DataContext context) : Controller
	{
		private readonly DataContext context = context;

		public async Task<IActionResult> Index(long id = 1)
		{
			ViewBag.AveragePrice = await context.Products.AverageAsync(p => p.Price);
			return View(await context.Products.FindAsync(id));
		}

		public IActionResult Common() => View();

		public IActionResult List() => View(context.Products);

		/// <summary>
		/// Envia propositadamente uma string com HTML dentro, de forma a simular uma entrada que poderia
		/// ser maliciosa por parte de um usuário tentando injetar código no processamento do resultado.
		/// 
		/// O cast para object é necessário, pois passar uma string como parâmetro para o métod View(),
		/// faria com que essa string fosse interpretada como o o nome da view que deveria ser carregada
		/// e não um parâmetro que deve ser usado como os dados processados na view.
		/// </summary>
		/// <returns></returns>
		public IActionResult Html() => View((object) "This is a <h3><i>string</i></h3>");
	}
}