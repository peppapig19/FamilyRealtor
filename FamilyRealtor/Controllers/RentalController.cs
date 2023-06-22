using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;
using FamilyRealtor.Models;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ViewModels;

namespace FamilyRealtor.Controllers
{
	public class RentalController : Controller
	{
		readonly RealtorContext _ctx;
		readonly UserManager<User> _userMngr;
		readonly SignInManager<User> _signInMngr;

		public RentalController(RealtorContext ctx, UserManager<User> userMngr, SignInManager<User> signInMngr)
		{
			_ctx = ctx;
			_userMngr = userMngr;
			_signInMngr = signInMngr;
		}

		[HttpGet]
		[Route("/[controller]s/[action]")]
		public async Task<IActionResult> DetailsAsync(int id)
		{
			Rental? rental = await _ctx.Rentals.FindAsync(id);
			if (rental == null) return NotFound();

			rental.AverageRating = await _ctx.Reviews.Where(rv => rv.Rental == rental).AverageAsync(rv => rv.Rating) ?? 0;
			rental.ReviewCount = await _ctx.Reviews.CountAsync(rv => rv.Rental == rental);

			Client? client = null;

			if (_signInMngr.IsSignedIn(User))
			{
				User? user = await _userMngr.GetUserAsync(User);
				client = user?.Client;
			}

			ReviewViewModel reviews = new()
			{
				RentalId = rental.Id,
				Reviews = await _ctx.Reviews.Where(r => r.Rental == rental).OrderByDescending(r => r.Id).ToListAsync(),
				AllowUserComment = client != null
					&& client.Bookings?.Any(b => b.Rental == rental) == true
					&& client.Reviews?.Any(r => r.Rental == rental) == false
			};

			ViewData["ReviewSection"] = reviews;
			return View(rental);
		}
	}
}