using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "администратор, риэлтор")]
	public class ReviewController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public ReviewController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/[controller]s")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Reviews.CountPagesAsync(_pageSize);
			List<Review> reviews = await _ctx.Reviews.OrderByDescending(r => r.Id).GetPage(page, _pageSize).ToListAsync();

			return View(reviews);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> SearchAsync(int sStars, int page = 1)
		{
			IQueryable<Review> query = _ctx.Reviews.Where(r => r.Rating == sStars);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Review> reviews = await query.OrderByDescending(r => r.Id).GetPage(page, _pageSize).ToListAsync();

			return View("Index", reviews);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Review? review = await _ctx.Reviews.FindAsync(id);
			if (review == null) return NotFound();

			_ctx.Reviews.Remove(review);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Review", new { Area = "Admin" }));
		}
	}
}