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
	public class BookingController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public BookingController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/[controller]s")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			IQueryable<Booking> query = _ctx.Bookings.Where(b => b.Paid != null);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Booking> bookings = await query.OrderByDescending(b => b.Paid).GetPage(page, _pageSize).ToListAsync();

			return View(bookings);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> SearchAsync(int sRental, int page = 1)
		{
			IQueryable<Booking> query = _ctx.Bookings.Where(b => b.Rental!.Id == sRental && b.Paid != null);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Booking> bookings = await query.OrderByDescending(b => b.Paid).GetPage(page, _pageSize).ToListAsync();

			return View("Index", bookings);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> Report(string reportDateFrom, string reportDateTo)
		{
			DateTime min = DateTime.Parse(reportDateFrom);
			DateTime max = DateTime.Parse(reportDateTo).AddDays(1);

			ViewData["MinDate"] = min;
			ViewData["MaxDate"] = max;

			IQueryable<Booking> query = _ctx.Bookings.Where(b => b.Paid >= min && b.Paid <= max);
			ViewData["Sum"] = await query.Select(b => b.TotalPrice).SumAsync();

			return View(await query.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Booking? booking = await _ctx.Bookings.FindAsync(id);
			if (booking == null) return NotFound();

			_ctx.Bookings.Remove(booking);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Booking", new { Area = "Admin" }));
		}
	}
}