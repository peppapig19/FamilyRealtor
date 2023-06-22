using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FamilyRealtor.Data;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ViewModels;

namespace FamilyRealtor.Controllers
{
	public class HomeController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public HomeController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/")]
		public async Task<IActionResult> IndexAsync(string? sCountry, int? sCity, int page = 1)
		{
			IQueryable<Rental> query = _ctx.Rentals.Where(r => r.IsVisible).OrderByDescending(r => r.TimeModified);

			if (sCountry != null) query = query.Where(r => r.City!.Country.Id == sCountry);
			if (sCity != null) query = query.Where(r => r.City!.Id == sCity);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Rental> rentals = await query.GetPage(page, _pageSize).ToListAsync();

			foreach (Rental rental in rentals)
			{
				rental.AverageRating = await _ctx.Reviews.Where(rv => rv.Rental == rental).AverageAsync(rv => rv.Rating) ?? 0;
				rental.ReviewCount = await _ctx.Reviews.CountAsync(rv => rv.Rental == rental);
			}

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View(rentals);
		}

		[HttpPost]
		[Route("/")]
		public async Task<JsonResult> Index(FilterViewModel filter)
		{
			HttpContext.Session.SetString("Filter", JsonSerializer.Serialize(filter));

			if (ModelState.IsValid) return Json(new
			{
				valid = true,
				content = Url.Action("Results", new { sCountry = filter.CountryId, sCity = filter.CityId })
			});
			else
			{
				await CommonLists.LoadListsAsync(this, _ctx, categories: true, facilities: true);
				await CommonLists.LoadDropDownListsAsync(this, _ctx, countries: true);

				return Json(new
				{
					valid = false,
					content = await ViewRenderer.RenderViewToStringAsync(this, "_FilterPartial", filter, isPartial: true)
				});
			}
		}

		[HttpGet]
		[Route("/[action]")]
		public async Task<IActionResult> Results(string sCountry, int? sCity, int page = 1)
		{
			if (!HttpContext.Session.TryGetValue("Filter", out byte[]? value)) RedirectToAction("Index");

			FilterViewModel? filter = JsonSerializer.Deserialize<FilterViewModel>(value);
			if (filter == null) return RedirectToAction("Index");

			IQueryable<Rental> query = _ctx.Rentals.Where(r => r.IsVisible).OrderByDescending(r => r.TimeModified);

			if (sCountry != null) query = query.Where(r => r.City!.Country.Id == sCountry);
			if (sCity != null) query = query.Where(r => r.City!.Id == sCity);

			query = query.Where(r => r.Bookings == null ||
					r.Bookings.All(b => b.CheckOutDate < filter.CheckInDate || b.CheckInDate > filter.CheckOutDate));

			if (filter.CategoryIds != null && filter.CategoryIds.Count != _ctx.Categories.Count())
				query = query.Where(r => filter.CategoryIds!.Any(cid => cid == r.Category!.Id));

			if (filter.FacilityIds != null)
			{
				foreach (int fid in filter.FacilityIds)
				{
					query = query.Where(r => r.Facilities!.Any(f => f.Id == fid));
				}
			}

			query = query.Where(r => r.MaximumGuests >= filter.Guests);

			if (filter.MinPriceADay != null) query = query
					.Where(r => Math.Round(r.PriceADay!.Value - (r.PriceADay.Value * (r.Discount != null ? (decimal)r.Discount / 100 : 0)))
					>= filter.MinPriceADay);

			if (filter.MaxPriceADay != null) query = query
					.Where(r => Math.Round(r.PriceADay!.Value - (r.PriceADay.Value * (r.Discount != null ? (decimal)r.Discount / 100 : 0)))
					<= filter.MaxPriceADay);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Rental> rentals = await query.GetPage(page, _pageSize).ToListAsync();

			foreach (Rental rental in rentals)
			{
				rental.AverageRating = await _ctx.Reviews.Where(rv => rv.Rental == rental).AverageAsync(rv => rv.Rating) ?? 0;
				rental.ReviewCount = await _ctx.Reviews.CountAsync(rv => rv.Rental == rental);
			}

			await CommonLists.LoadListsAsync(this, _ctx, countries: true);
			return View("Index", rentals);
		}

		[Route("[action]")]
		public IActionResult About()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> GetFilterPartialAsync(string? countryId, int? cityId)
		{
			await CommonLists.LoadListsAsync(this, _ctx, categories: true, facilities: true);
			await CommonLists.LoadDropDownListsAsync(this, _ctx, countries: true);

			if (HttpContext.Session.TryGetValue("Filter", out byte[]? value))
			{
				FilterViewModel? filter = JsonSerializer.Deserialize<FilterViewModel>(value);
				if (filter != null) return PartialView("_FilterPartial", filter);
			}

			if (countryId != null && cityId != null)
			{
				FilterViewModel vm = new() { CountryId = countryId, CityId = cityId };
				return PartialView("_FilterPartial", vm);
			}

			return PartialView("_FilterPartial");
		}

		[HttpGet]
		public async Task<JsonResult> GetCitiesByCountryAsync(string countryId)
		{
			List<City> cityList = await _ctx.Cities.Where(c => c.Country.Id == countryId).OrderBy(c => c.Name).ToListAsync();
			return Json(cityList);
		}
	}
}