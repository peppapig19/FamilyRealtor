using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Facility")]
	public class Facility
	{
		public int Id { get; set; }

		[CustomRequired]
		[StringLength(50)]
		[Remote("CheckFacilityUnique", "Validation", AdditionalFields = "Id")]
		[DisplayName("Название:")]
		public string Name { get; set; } = null!;

		[DisplayName("Класс иконки:")]
		public string? IconClass { get; set; }

		[JsonIgnore]
		public virtual List<Rental>? Rentals { get; set; }
    }
}