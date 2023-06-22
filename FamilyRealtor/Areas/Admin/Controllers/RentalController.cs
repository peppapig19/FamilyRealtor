using BingMapsRESTToolkit;
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
	public class RentalController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;
		readonly IWebHostEnvironment _env;

		public RentalController(RealtorContext ctx, IConfiguration conf, IWebHostEnvironment env)
		{
			_ctx = ctx;
			_conf = conf;
			_env = env;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/[controller]s")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Rentals.CountPagesAsync(_pageSize);
			List<Rental> rentals = await _ctx.Rentals.OrderByDescending(r => r.TimeModified).GetPage(page, _pageSize).ToListAsync();

			await CommonLists.LoadListsAsync(this, _ctx, categories: true, countries: true);
			return View(rentals);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> SearchAsync(string? sName, string? sAddress, string? sCat, string? sCountry, int? sCity, int page = 1)
		{
			IQueryable<Rental> query = _ctx.Rentals;

			if (sName != null) query = query.Where(r => r.Name.ToLower().Contains(sName.ToLower()));
			if (sAddress != null) query = query.Where(r => (r.Street + ", " + r.House + ", " + r.Apartment).ToLower().Contains(sAddress.ToLower()));
			if (sCat != null) query = query.Where(r => r.Category!.Name.ToLower() == sCat.ToLower());
			if (sCountry != null) query = query.Where(r => r.City!.Country.Id == sCountry);
			if (sCity != null) query = query.Where(r => r.City!.Id == sCity);

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Rental> rentals = await query.OrderByDescending(r => r.TimeModified).GetPage(page, _pageSize).ToListAsync();

			await CommonLists.LoadListsAsync(this, _ctx, categories: true, countries: true);
			return View("Index", rentals);
		}

		[HttpGet]
		public async Task<IActionResult> CreateAsync()
		{
			await CommonLists.LoadListsAsync(this, _ctx, facilities: true);
			await CommonLists.LoadDropDownListsAsync(this, _ctx, categories: true, countries: true);
			return View("Form");
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync(Rental rental)
		{
			if (ModelState.IsValid)
			{
				if (rental.FacilityIds != null)
				{
					rental.Facilities = new();

					foreach (int id in rental.FacilityIds)
					{
						Facility? facility = await _ctx.Facilities.FindAsync(id);
						if (facility != null) rental.Facilities.Add(facility);
					}
				}

				_ctx.Rentals.Add(rental);
				await _ctx.SaveChangesAsync();

				await _ctx.Entry(rental).Reference(r => r.City).LoadAsync();
				await GeocodeAsync(rental);
				await UploadPhotosAsync(rental);

				return RedirectToAction("Index");
			}

			await CommonLists.LoadListsAsync(this, _ctx, facilities: true);
			await CommonLists.LoadDropDownListsAsync(this, _ctx, categories: true, countries: true);
			return View("Form", rental);
		}

		[HttpGet]
		public async Task<IActionResult> UpdateAsync(int id)
		{
			Rental? rental = await _ctx.Rentals.FindAsync(id);
			if (rental == null) return NotFound();

			rental.CountryId = rental.City!.CountryId;

			await CommonLists.LoadListsAsync(this, _ctx, facilities: true);
			await CommonLists.LoadDropDownListsAsync(this, _ctx, categories: true, countries: true);
			return View("Form", rental);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(Rental model)
		{
			if (ModelState.IsValid)
			{
				Rental? rental = await _ctx.Rentals.FindAsync(model.Id);
				if (rental == null) return NotFound();

				rental.Category = await _ctx.Categories.FindAsync(model.CategoryId);
				rental.Facilities?.Clear();

				if (model.FacilityIds != null)
				{
					rental.Facilities = new();

					foreach (int id in model.FacilityIds)
					{
						Facility? facility = await _ctx.Facilities.FindAsync(id);
						if (facility != null) rental.Facilities.Add(facility);
					}
				}

				rental.TimeModified = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

				_ctx.Rentals.Update(rental);
				await _ctx.SaveChangesAsync();

				await _ctx.Entry(rental).Reference(r => r.City).LoadAsync();
				await GeocodeAsync(rental);
				await UploadPhotosAsync(model);

				return RedirectToAction("Index");
			}

			await CommonLists.LoadListsAsync(this, _ctx, facilities: true);
			await CommonLists.LoadDropDownListsAsync(this, _ctx, categories: true, countries: true);
			return View("Form", model);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Rental? rental = await _ctx.Rentals
				.Include(r => r.Facilities)
				.Include(r => r.Photos)
				.Include(r => r.Bookings)
				.Include(r => r.Reviews)
				.FirstOrDefaultAsync(r => r.Id == id);
			if (rental == null) return NotFound();

			_ctx.Rentals.Remove(rental);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Rental", new { Area = "Admin" }));
		}

		async Task GeocodeAsync(Rental rental)
		{
			if (rental.Id == 0) throw new ArgumentNullException(nameof(rental), "Объект аренды ещё не создан.");

			SimpleAddress address = new()
			{
				CountryRegion = rental.City!.CountryId,
				Locality = rental.City!.Name.ToString(),
				AddressLine = rental.Street
			};

			GeocodeRequest request = new()
			{
				Address = address,
				MaxResults = 1,
				BingMapsKey = _conf["ApiKeys:BingMaps"]
			};

			Response response = await request.Execute();

			if (response.ResourceSets[0].Resources.Length > 0)
			{
				Location? location = response.ResourceSets[0].Resources[0] as Location;
				Point? point = location?.Point;

				if (point != null)
				{
					rental.Latitude = point.Coordinates[0];
					rental.Longitude = point.Coordinates[1];

					_ctx.Rentals.Update(rental);
					await _ctx.SaveChangesAsync();
				}
			}
		}

		async Task UploadPhotosAsync(Rental model)
		{
			Rental? rental = await _ctx.Rentals.FindAsync(model.Id)
				?? throw new ArgumentNullException(nameof(model), "Объект аренды ещё не создан.");

			rental.Photos?.Clear();

			if (model.FormFiles != null)
			{
				rental.Photos = new();

				foreach (IFormFile file in model.FormFiles)
				{
					string fileName = Path.GetFileName(file.FileName);
					string path = Path.Combine(_env.WebRootPath, $"Images/Rent/{rental.Id}/");

					Directory.CreateDirectory(path);
					path += fileName;

					using (FileStream fileStream = new(path, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					Photo photo = new() { Rental = rental, Path = fileName };
					_ctx.Photos.Add(photo);
				}
			}

			await _ctx.SaveChangesAsync();
		}
	}
}