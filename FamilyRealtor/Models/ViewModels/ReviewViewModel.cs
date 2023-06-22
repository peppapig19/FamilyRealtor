using System.ComponentModel.DataAnnotations;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models.ViewModels
{
	public class ReviewViewModel
	{
		public List<Review>? Reviews { get; set; }

		public int RentalId { get; set; }

		public bool AllowUserComment { get; set; }

		[Required(ErrorMessage = "Пожалуйста, поставьте оценку от 1 до 5.")]
		public int? Rating { get; set; }

		[Required(ErrorMessage = "Пожалуйста, обоснуйте оценку.")]
		[StringLength(500)]
		public string? Content { get; set; }
	}
}