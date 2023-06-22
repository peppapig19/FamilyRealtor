using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;

namespace FamilyRealtor.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ValidationController : Controller
	{
		readonly RealtorContext _ctx;

		public ValidationController(RealtorContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<JsonResult> CheckRentalUnique(string name, int id)
		{
			if (!string.IsNullOrEmpty(name))
			{
				bool exists;

				if (id == 0) exists = await _ctx.Rentals.AnyAsync(r => r.Name.ToLower() == name.ToLower());
				else exists = await _ctx.Rentals.AnyAsync(r => r.Name.ToLower() == name.ToLower() && r.Id != id);

				if (exists) return Json("Объект аренды с таким названием уже существует.");
			}

			return Json(true);
		}

		public async Task<JsonResult> CheckCategoryUnique(string name, int id)
		{
			if (!string.IsNullOrEmpty(name))
			{
				bool exists;

				if (id == 0) exists = await _ctx.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
				else exists = await _ctx.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != id);

				if (exists) return Json("Категория с таким названием уже существует.");
			}

			return Json(true);
		}

		public async Task<JsonResult> CheckFacilityUnique(string name, int id)
		{
			if (!string.IsNullOrEmpty(name))
			{
				bool exists;

				if (id == 0) exists = await _ctx.Facilities.AnyAsync(f => f.Name.ToLower() == name.ToLower());
				else exists = await _ctx.Facilities.AnyAsync(f => f.Name.ToLower() == name.ToLower() && f.Id != id);

				if (exists) return Json("Удобство с таким названием уже существует.");
			}

			return Json(true);
		}

		public async Task<JsonResult> CheckCountryUnique(string name, string id)
		{
			if (!string.IsNullOrEmpty(name))
			{
				bool exists;

				if (string.IsNullOrEmpty(id)) exists = await _ctx.Countries.AnyAsync(c => c.Name.ToLower() == name.ToLower());
				else exists = await _ctx.Countries.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != id);

				if (exists) return Json("Страна с таким названием уже существует.");
			}

			return Json(true);
		}

		public async Task<JsonResult> CheckCityUnique(string name, string countryId, int id)
		{
			if (!string.IsNullOrEmpty(name))
			{
				bool exists;

				if (id == 0) exists = await _ctx.Cities.AnyAsync(c => c.Country.Id == countryId && c.Name.ToLower() == name.ToLower());
				else exists = await _ctx.Cities.AnyAsync(c => c.Country.Id == countryId && c.Name.ToLower() == name.ToLower() && c.Id != id);

				if (exists) return Json("Город с таким названием уже существует в стране.");
			}

			return Json(true);
		}
	}
}