using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.ViewModels
{
	public class FilterViewModel
	{
		[Required(ErrorMessage = "Пожалуйста, выберите страну.")]
		[DisplayName("Страна:")]
		public string CountryId { get; set; } = null!;

		[DisplayName("Город:")]
		public int? CityId { get; set; }

		[Required(ErrorMessage = "Пожалуйста, укажите дату заселения.")]
		[DisplayName("Дата заселения:")]
		public DateOnly? CheckInDate { get; set; }

		[Required(ErrorMessage = "Пожалуйста, укажите дату выселения.")]
		[GreaterThan(nameof(CheckInDate))]
		[DisplayName("Дата выселения:")]
		public DateOnly? CheckOutDate { get; set; }

		[DisplayName("Категория:")]
		public List<int>? CategoryIds { get; set; }

		[DisplayName("Удобства:")]
		public List<int>? FacilityIds { get; set; }

		[Required(ErrorMessage = "Пожалуйста, выберите количество гостей.")]
		[PositiveNumber]
		[Range(1, 10, ErrorMessage = "Гостей может быть от 1 до 10.")]
		[DisplayName("Гостей:")]
		public int? Guests { get; set; }

		[PositiveNumber]
		[DisplayName("Цена за сутки, от:")]
		public int? MinPriceADay { get; set; }

		[PositiveNumber]
		[GreaterThan(nameof(MinPriceADay))]
		[DisplayName("Цена за сутки, до:")]
		public int? MaxPriceADay { get; set; }
	}
}