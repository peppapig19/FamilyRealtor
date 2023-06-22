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
	public class ClientController : Controller
	{
		readonly int _pageSize;

		readonly RealtorContext _ctx;
		readonly IConfiguration _conf;

		public ClientController(RealtorContext ctx, IConfiguration conf)
		{
			_ctx = ctx;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
		}

		[HttpGet]
		[Route("/[area]/[controller]s")]
		public async Task<IActionResult> IndexAsync(int page = 1)
		{
			ViewData["PageCount"] = await _ctx.Clients.CountPagesAsync(_pageSize);
			List<Client> clients = await _ctx.Clients.OrderByDescending(c => c.Id).GetPage(page, _pageSize).ToListAsync();

			return View(clients);
		}

		[HttpGet]
		[Route("/[area]/[controller]s/[action]")]
		public async Task<IActionResult> SearchAsync(string? sName, string? sPhone, int page = 1)
		{
			IQueryable<Client> query = _ctx.Clients;

			if (sName != null) query = query.Where(c => (c.LastName + " " + c.FirstName + " " + c.Patronymic).ToLower().Contains(sName.ToLower()));
			if (sPhone != null) query = query.Where(c => c.Phone!.Contains(sPhone));

			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Client> clients = await query.OrderByDescending(c => c.Id).GetPage(page, _pageSize).ToListAsync();

			return View("Index", clients);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			Client? client = await _ctx.Clients
				.Include(c => c.User)
				.Include(c => c.Bookings)
				.Include(c => c.Reviews)
				.FirstOrDefaultAsync(c => c.Id == id);
			if (client == null) return NotFound();

			_ctx.Clients.Remove(client);
			await _ctx.SaveChangesAsync();

			return Json(Url.Action("Index", "Client", new { Area = "Admin" }));
		}
	}
}