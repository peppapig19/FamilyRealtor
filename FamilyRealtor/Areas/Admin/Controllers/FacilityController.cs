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
	public class FacilityController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public FacilityController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/Facilities")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Facilities.CountPagesAsync(_pageSize);
			List<Facility> facilities = await _ctx.Facilities.OrderBy(f => f.Name).GetPage(page, _pageSize).ToListAsync();

			return View(facilities);
		}

		[HttpGet]
		[Route("/[area]/Facilities/[action]")]
		public async Task<IActionResult> SearchAsync(string sName, int page = 1)
		{
			IQueryable<Facility> query = _ctx.Facilities.Where(f => f.Name.ToLower().Contains(sName.ToLower()));

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Facility> facilities = await query.OrderBy(f => f.Name).GetPage(page, _pageSize).ToListAsync();

			return View("Index", facilities);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Facility facility)
		{
			if (ModelState.IsValid)
			{
				_ctx.Facilities.Add(facility);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View("Form", facility);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAsync(int id)
		{
			Facility? facility = await _ctx.Facilities.FindAsync(id);
			if (facility == null) return NotFound();

			return View("Form", facility);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(Facility facility)
		{
			if (ModelState.IsValid)
			{
				_ctx.Facilities.Update(facility);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return View("Form", facility);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Facility? facility = await _ctx.Facilities
				.Include(f => f.Rentals)
				.FirstOrDefaultAsync(f => f.Id == id);
			if (facility == null) return NotFound();

			_ctx.Facilities.Remove(facility);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Facility", new { Area = "Admin" }));
		}
	}
}