using Microsoft.AspNetCore.Mvc;

namespace FamilyRealtor.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult GetDeletePartial(string name)
		{
			return PartialView("_DeletePartial", name);
		}
	}
}