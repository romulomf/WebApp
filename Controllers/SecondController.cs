using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class SecondController : Controller
	{
		public IActionResult Index() => View("Common");
	}
}
