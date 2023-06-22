using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;
using FamilyRealtor.Models;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ViewModels;

namespace FamilyRealtor.Controllers
{
	public class ReviewController : Controller
	{
		readonly RealtorContext _ctx;
		readonly UserManager<User> _userMngr;
		readonly SignInManager<User> _signInMngr;

		public ReviewController(RealtorContext ctx, UserManager<User> userMngr, SignInManager<User> signInMngr)
		{
			_ctx = ctx;
			_userMngr = userMngr;
			_signInMngr = signInMngr;
		}

		[HttpPost]
		public async Task<IActionResult> Comment(ReviewViewModel vm)
		{
			Client? client = null;

			if (_signInMngr.IsSignedIn(User))
			{
				User? user = await _userMngr.GetUserAsync(User);
				client = user?.Client;
			}

			if (client == null) return Unauthorized();

			Rental? rental = await _ctx.Rentals.FindAsync(vm.RentalId);
			if (rental == null) return BadRequest();

			bool AllowUserComment = client.Bookings?.Any(b => b.Rental == rental) == true && client.Reviews?.Any(r => r.Rental == rental) == false;
			if (!AllowUserComment) return BadRequest();

			if (ModelState.IsValid)
			{
				Review review = new()
				{
					Client = client,
					Rental = rental,
					Rating = vm.Rating,
					Content = vm.Content
				};

				_ctx.Reviews.Add(review);
				await _ctx.SaveChangesAsync();

				vm.AllowUserComment = false;
			}

			vm.Reviews = await _ctx.Reviews.Where(r => r.Rental == rental).ToListAsync();
			return PartialView("_ReviewsPartial", vm);
		}
	}
}