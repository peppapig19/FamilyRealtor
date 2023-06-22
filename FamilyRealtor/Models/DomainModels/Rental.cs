using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Rental")]
	public class Rental
	{
		public int Id { get; set; }

		[JsonIgnore]
		public virtual Category? Category { get; set; }

		[DisplayName("Категория:")]
		public int? CategoryId { get; set; }

		[Required(ErrorMessage = "Пожалуйста, выберите город.")]
		[DisplayName("Город:")]
		public int? CityId { get; set; }
		public virtual City? City { get; set; }

		[CustomRequired]
		[StringLength(100)]
		[Remote("CheckRentalUnique", "Validation", AdditionalFields = "Id")]
		[DisplayName("Название:")]
		public string Name { get; set; } = null!;

		[CustomRequired]
		[StringLength(100)]
		[DisplayName("Улица:")]
		public string Street { get; set; } = null!;

		[CustomRequired]
		[StringLength(10)]
		[DisplayName("Дом:")]
		public string House { get; set; } = null!;

		[StringLength(10)]
		[DisplayName("Квартира (при наличии):")]
		public string? Apartment { get; set; }

		public double? Latitude { get; set; }

		public double? Longitude { get; set; }

		[CustomRequired]
		[PositiveNumber]
		[Range(1, 10, ErrorMessage = "Гостей может быть от 1 до 10.")]
		[DisplayName("Гостей до:")]
		public int? MaximumGuests { get; set; }

		[DisplayName("Описание:")]
		public string? Description { get; set; }

		[CustomRequired]
		[PositiveNumber]
		[Precision(18, 0)]
		[DisplayFormat(DataFormatString = "{0:f0}", ApplyFormatInEditMode = true)]
		[DisplayName("Цена за сутки:")]
		public decimal? PriceADay { get; set; }

		[PositiveNumber]
		[Range(0, 100, ErrorMessage = "Скидка не может быть больше 100%.")]
		[DisplayName("Скидка, % (при наличии):")]
		public int? Discount { get; set; }

		[DisplayName("Последнее изменение:")]
		public DateTime? TimeModified { get; set; }

		[DisplayName("Отображение в списке предложений")]
		public bool IsVisible { get; set; }

		[JsonIgnore]
		public virtual List<Facility>? Facilities { get; set; }

		[JsonIgnore]
		public virtual List<Photo>? Photos { get; set; }

		[JsonIgnore]
		public virtual List<Booking>? Bookings { get; set; }

		[JsonIgnore]
		public virtual List<Review>? Reviews { get; set; }

		[NotMapped]
		[CustomRequired]
		public string CountryId { get; set; } = null!;

		[NotMapped]
		public List<int>? FacilityIds { get; set; }

		[NotMapped]
		public double AverageRating { get; set; }

		[NotMapped]
		public int ReviewCount { get; set; }

		[NotMapped]
		[DisplayName("Изображения:")]
		public IFormFileCollection? FormFiles { get; set; }

		[DisplayName("Цена с учётом скидки:")]
		public decimal FinalPriceADay => Discount != null
			? Math.Ceiling(PriceADay!.Value - (PriceADay.Value * ((decimal)Discount / 100)))
			: PriceADay!.Value;

		public string StreetAddress => $"{Street}, {(Apartment == null ? House : House + ", " + Apartment)}";

		public string FullAddress => $"{City?.Country.Name}, {City?.Name}, {StreetAddress}";
	}
}