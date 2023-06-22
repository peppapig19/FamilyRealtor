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
	public class CountryController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public CountryController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/Countries")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Countries.CountPagesAsync(_pageSize);
			List<Country> countries = await _ctx.Countries.OrderBy(c => c.Name).GetPage(page, _pageSize).ToListAsync();

			return View(countries);
		}

		[HttpGet]
		[Route("/[area]/Countries/[action]")]
		public async Task<IActionResult> SearchAsync(string sName, int page = 1)
		{
			IQueryable<Country> query = _ctx.Countries.Where(c => c.Name.ToLower().Contains(sName.ToLower()));

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Country> countries = await query.OrderBy(c => c.Name).GetPage(page, _pageSize).ToListAsync();

			return View("Index", countries);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Country country)
		{
			if (ModelState.IsValid)
			{
				_ctx.Countries.Add(country);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View("Form", country);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAsync(string id)
		{
			Country? country = await _ctx.Countries.FindAsync(id);
			if (country == null) return NotFound();

			return View("Form", country);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(Country country)
		{
			if (ModelState.IsValid)
			{
				_ctx.Countries.Update(country);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View("Form", country);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(string id)
		{
			Country? country = await _ctx.Countries
				.Include(c => c.Cities)
				.FirstOrDefaultAsync(c => c.Id == id);
			if (country == null) return NotFound();

			_ctx.Countries.Remove(country);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Country", new { Area = "Admin" }));
		}
	}
}