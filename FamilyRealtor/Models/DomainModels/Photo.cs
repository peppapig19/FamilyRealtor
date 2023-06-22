using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Photo")]
	public class Photo
	{
		public int Id { get; set; }

		public virtual Rental? Rental { get; set; }

		public string Path { get; set; } = null!;
	}
}