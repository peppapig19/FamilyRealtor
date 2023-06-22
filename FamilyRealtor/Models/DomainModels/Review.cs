using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Review")]
	public class Review
	{
		public int Id { get; set; }

		[JsonIgnore]
		public virtual Client Client { get; set; } = null!;

		[JsonIgnore]
		public virtual Rental Rental { get; set; } = null!;

		[Required(ErrorMessage = "Пожалуйста, поставьте оценку от 1 до 5.")]
		public int? Rating { get; set; }

		[Required(ErrorMessage = "Пожалуйста, обоснуйте оценку.")]
		[StringLength(500)]
		public string? Content { get; set; }
	}
}