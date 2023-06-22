using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FamilyRealtor.Data;
using FamilyRealtor.Helpers;
using FamilyRealtor.Models;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ViewModels;
using Yandex.Checkout.V3;
using Client = FamilyRealtor.Models.DomainModels.Client;
using YMClient = Yandex.Checkout.V3.Client;

namespace FamilyRealtor.Controllers
{
	public class BookingController : Controller
	{
		readonly int _pageSize;
		readonly YMClient _client;

		readonly RealtorContext _ctx;
		readonly UserManager<User> _userMngr;
		readonly SignInManager<User> _signInMngr;
		readonly IConfiguration _conf;

		public BookingController(RealtorContext ctx, UserManager<User> userMngr, SignInManager<User> signInMngr, IConfiguration conf)
		{
			_ctx = ctx;
			_userMngr = userMngr;
			_signInMngr = signInMngr;
			_conf = conf;

			_pageSize = Convert.ToInt32(_conf["PageSize"]);
			_client = new(shopId: "317480", secretKey: _conf["ApiKeys:YooMoney"]);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> MyBookings(int page = 1)
		{
			Client? client = null;

			if (_signInMngr.IsSignedIn(User))
			{
				User? user = await _userMngr.GetUserAsync(User);
				client = user?.Client;
			}

			IQueryable<Booking> query = _ctx.Bookings.Where(b => b.Client == client && b.Paid != null);
			ViewData["PageCount"] = await query.CountPagesAsync(_pageSize);
			List<Booking> bookings = await query.OrderByDescending(b => b.Id).GetPage(page, _pageSize).ToListAsync();

			return View(bookings);
		}

		[HttpPost]
		public async Task<JsonResult> Pay(Booking booking)
		{
			if (_signInMngr.IsSignedIn(User))
			{
				User? user = await _userMngr.GetUserAsync(User);
				booking.Client = user?.Client;
			}

			if (booking.Client == null) return Json(new
			{
				valid = true,
				content = Url.Action("Login", "Account")
			});

			booking.Rental = await _ctx.Rentals.FindAsync(booking.RentalId);
			if (booking.Rental == null) return Json(null);

			if (ModelState.IsValid)
			{
				booking.TotalPrice = booking.Rental.FinalPriceADay * booking.Days;

				_ctx.Bookings.Add(booking);
				await _ctx.SaveChangesAsync();

				NewPayment newPayment = new()
				{
					Amount = new Amount { Value = booking.TotalPrice, Currency = "RUB" },
					Confirmation = new()
					{
						Type = ConfirmationType.Redirect,
						ReturnUrl = Url.Action("Paid", "Booking", new { id = booking.Id }, "https")
					}
				};

				Payment payment = _client.CreatePayment(newPayment);
				booking.PaymentId = payment.Id;

				_ctx.Bookings.Update(booking);
				await _ctx.SaveChangesAsync();

				return Json(new
				{
					valid = true,
					content = payment.Confirmation.ConfirmationUrl
				});
			}
			else return Json(new
			{
				valid = false,
				content = await ViewRenderer.RenderViewToStringAsync(this, "_BookingFormPartial", booking, isPartial: true)
			});
		}

		[HttpGet]
		public async Task<IActionResult> Paid(int id)
		{
			Booking? booking = await _ctx.Bookings.FindAsync(id);
			string? paymentId = booking?.PaymentId?.ToString();
			if (booking == null || paymentId == null) return NotFound();

			if (_client.GetPayment(paymentId).Paid)
			{
				_client.CapturePayment(paymentId);
				booking.Paid = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

				_ctx.Bookings.Update(booking);
				await _ctx.SaveChangesAsync();

				return RedirectToAction("MyBookings");
			}
			else return Content("При обработке платежа произошла ошибка.");
		}

		[HttpPost]
		public async Task<IActionResult> GetBookingPartial(int rentalId)
		{
			Booking booking = new()
			{
				Rental = await _ctx.Rentals.FindAsync(rentalId),
				RentalId = rentalId
			};

			FilterViewModel? filter = null;
			if (HttpContext.Session.TryGetValue("Filter", out byte[]? value)) filter = JsonSerializer.Deserialize<FilterViewModel>(value);

			if (filter != null)
			{
				booking.CheckInDate = filter.CheckInDate;
				booking.CheckOutDate = filter.CheckOutDate;
				booking.Guests = filter.Guests;
			};

			return PartialView("_BookingFormPartial", booking);
		}

		[HttpGet]
		public async Task<JsonResult> GetBookedDates(int rentalId)
		{
			List<Booking> bookings = await _ctx.Bookings.Where(b => b.Rental!.Id == rentalId && b.Paid != null).ToListAsync();
			List<string> bookedDates = new();

			foreach (Booking booking in bookings)
			{
				for (DateOnly date = booking.CheckInDate!.Value; date <= booking.CheckOutDate; date = date.AddDays(1))
				{
					bookedDates.Add(date.ToString("dd.MM.yyyy"));
				}
			}

			return Json(bookedDates);
		}
	}
}