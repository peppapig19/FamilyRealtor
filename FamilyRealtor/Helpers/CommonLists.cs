using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;

namespace FamilyRealtor.Helpers
{
	public static class CommonLists
	{
		public static async Task LoadListsAsync(Controller controller, RealtorContext ctx,
			bool categories = false, bool facilities = false, bool countries = false)
		{
			if (categories) controller.ViewData["Categories"] = await ctx.Categories.OrderBy(c => c.Name).ToListAsync();
			if (facilities) controller.ViewData["Facilities"] = await ctx.Facilities.OrderBy(f => f.Name).ToListAsync();
			if (countries) controller.ViewData["Countries"] = await ctx.Countries.OrderBy(c => c.Name).ToListAsync();
		}

		public static async Task LoadDropDownListsAsync(Controller controller, RealtorContext ctx, bool categories = false, bool countries = false)
		{
			if (categories) controller.ViewData["CategoryDropdown"] = new SelectList(
				await ctx.Categories.OrderBy(c => c.Name).ToListAsync(),
				"Id", "Name");

			if (countries) controller.ViewData["CountryDropdown"] = new SelectList(
				await ctx.Countries.OrderBy(c => c.Name).ToListAsync(),
				"Id", "Name");
		}
	}
}