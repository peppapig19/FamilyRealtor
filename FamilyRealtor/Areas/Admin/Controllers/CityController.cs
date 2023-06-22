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
	public class CityController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public CityController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/Cities")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Cities.CountPagesAsync(_pageSize);
			List<City> cities = await _ctx.Cities.OrderBy(c => c.Name).GetPage(page, _pageSize).ToListAsync();

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View(cities);
		}

		[HttpGet]
		[Route("/[area]/Categories/[action]")]
		public async Task<IActionResult> SearchAsync(string? sName, string? sCountry, int page = 1)
		{
			IQueryable<City> query = _ctx.Cities;

			if (sName != null) query = query.Where(c => c.Name.ToLower().Contains(sName.ToLower()));
			if (sCountry != null) query = query.Where(c => c.Country.Id == sCountry);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<City> cities = await query.OrderBy(c => c.Name).GetPage(page, _pageSize).ToListAsync();

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Index", cities);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(City city)
		{
			if (ModelState.IsValid)
			{
				_ctx.Cities.Add(city);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Form", city);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAsync(int id)
		{
			City? city = await _ctx.Cities.FindAsync(id);
			if (city == null) return NotFound();

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Form", city);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(City city)
		{
			if (ModelState.IsValid)
			{
				_ctx.Cities.Update(city);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Form", city);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			City? city = await _ctx.Cities
				.Include(c => c.Rentals)
				.FirstOrDefaultAsync(c => c.Id == id);
			if (city == null) return NotFound();

			_ctx.Cities.Remove(city);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "City", new { Area = "Admin" }));
		}
	}
}