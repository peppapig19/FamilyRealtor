using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Country")]
	public class Country
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[StringLength(2)]
		public string Id { get; set; } = null!;

		[CustomRequired]
		[StringLength(50)]
		[Remote("CheckCountryUnique", "Validation", AdditionalFields = "Id")]
		[DisplayName("Название:")]
		public string Name { get; set; } = null!;

		[JsonIgnore]
		public virtual List<City>? Cities { get; set; }
	}
}