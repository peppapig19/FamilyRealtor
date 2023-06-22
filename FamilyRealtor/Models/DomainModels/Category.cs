using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Category")]
	public class Category
	{
		public int Id { get; set; }

		[CustomRequired]
		[StringLength(50)]
		[Remote("CheckCategoryUnique", "Validation", AdditionalFields = "Id")]
		[DisplayName("Название:")]
		public string Name { get; set; } = null!;

		[JsonIgnore]
		public virtual List<Rental>? Rentals { get; set; }
	}
}