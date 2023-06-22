using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyRealtor.Data;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Controllers
{
    public class ValidationController : Controller
    {
        readonly RealtorContext _ctx;

        public ValidationController(RealtorContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<JsonResult> CheckBookingOverlap(string checkInDate, string checkOutDate, int rentalId)
        {
            DateOnly cid = DateOnly.Parse(checkInDate);
            DateOnly cod = DateOnly.Parse(checkOutDate);

            List<Booking> bookings = await _ctx.Bookings.Where(b => b.Rental!.Id == rentalId && b.Paid != null).ToListAsync();

			bool hasOverlap = bookings.Any(b => b.CheckInDate >= cid && b.CheckInDate <= cod || b.CheckOutDate >= cid && b.CheckOutDate <= cod);

            if (hasOverlap) return Json("В выбранном периоде уже есть бронь.");
            else return Json(true);
        }
    }
}