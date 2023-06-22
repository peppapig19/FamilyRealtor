using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Booking")]
	public class Booking
	{
		public int Id { get; set; }

		[JsonIgnore]
		public virtual Rental? Rental { get; set; }

		public int? RentalId { get; set; }

		[JsonIgnore]
		public virtual Client? Client { get; set; }

		[Required(ErrorMessage = "Пожалуйста, укажите дату заселения.")]
		[DisplayName("Дата заселения:")]
		public DateOnly? CheckInDate { get; set; }

		[Required(ErrorMessage = "Пожалуйста, укажите дату выселения.")]
		[GreaterThan(nameof(CheckInDate))]
		[Remote("CheckBookingOverlap", "Validation", AdditionalFields = $"{nameof(CheckInDate)}, {nameof(RentalId)}")]
		[DisplayName("Дата выселения:")]
		public DateOnly? CheckOutDate { get; set; }

		[Required(ErrorMessage = "Пожалуйста, выберите количество гостей.")]
		[PositiveNumber]
		[Range(1, 10, ErrorMessage = "Гостей должно быть от 1 до 10.")]
		[DisplayName("Гостей:")]
		public int? Guests { get; set; }

		[Precision(18, 0)]
		[DisplayFormat(DataFormatString = "{0:f0}")]
		[DisplayName("Итоговая цена:")]
		public decimal TotalPrice { get; set; }

		[DisplayName("Номер платежа:")]
		public string? PaymentId { get; set; }

		public DateTime? Paid { get; set; }

		public int Days => DateOnly.TryParse(CheckInDate.ToString(), out DateOnly cid)
			&& DateOnly.TryParse(CheckOutDate.ToString(), out DateOnly cod)
			? (int)(new DateTime(cod.Year, cod.Month, cod.Day, 0, 0, 0) - new DateTime(cid.Year, cid.Month, cid.Day, 0, 0, 0)).TotalDays
			: 0;
	}
}