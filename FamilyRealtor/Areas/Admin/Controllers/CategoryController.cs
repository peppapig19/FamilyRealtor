using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "администратор")]
	public class CategoryController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public CategoryController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/Categories")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Categories.CountPagesAsync(_pageSize);
			List<Category> categories = await _ctx.Categories.OrderBy(c => c.Name).GetPage(page, _pageSize).ToListAsync();

			return View(categories);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Category category)
		{
			if (ModelState.IsValid)
			{
				_ctx.Categories.Add(category);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View("Form", category);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Category? category = await _ctx.Categories
				.Include(c => c.Rentals)
				.FirstOrDefaultAsync(c => c.Id == id);
			if (category == null) return NotFound();

			_ctx.Categories.Remove(category);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Category", new { Area = "Admin" }));
		}
	}
}