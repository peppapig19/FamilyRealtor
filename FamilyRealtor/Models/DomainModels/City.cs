using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("City")]
	public class City
	{
		public int Id { get; set; }

		[JsonIgnore]
		public virtual Country Country { get; set; } = null!;

		[Required(ErrorMessage = "Пожалуйста, выберите страну.")]
		public string CountryId { get; set; } = null!;

		[CustomRequired]
		[StringLength(50)]
		[Remote("CheckCityUnique", "Validation", AdditionalFields = $"{nameof(CountryId)}, Id")]
		[DisplayName("Название:")]
		public string Name { get; set; } = null!;

		[JsonIgnore]
		public virtual List<Rental>? Rentals { get; set; }
	}
}