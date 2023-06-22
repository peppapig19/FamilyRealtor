using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.DomainModels
{
	[Table("Client")]
	public class Client
	{
		public int Id { get; set; }

		[JsonIgnore]
		public virtual User? User { get; set; }

		[CustomRequired]
		[RegularExpression(@"^[а-яА-Я-]+$", ErrorMessage = "Пожалуйста, введите имя на русском.")]
		[StringLength(100)]
		[DisplayName("Имя:")]
		public string? FirstName { get; set; }

		[CustomRequired]
		[RegularExpression(@"^[а-яА-Я-]+$", ErrorMessage = "Пожалуйста, введите фамилию на русском.")]
		[StringLength(100)]
		[DisplayName("Фамилия:")]
		public string? LastName { get; set; }

		[RegularExpression(@"^[а-яА-Я-]*$", ErrorMessage = "Пожалуйста, введите отчество на русском.")]
		[StringLength(100)]
		[DisplayName("Отчество (при наличии):")]
		public string? Patronymic { get; set; }

		[CustomRequired]
		[AgeRange]
		[DisplayName("Дата рождения:")]
		public DateOnly? DOB { get; set; }

		[CustomRequired]
		[RegularExpression(@"^(\+7\d{10})$", ErrorMessage = "Пожалуйста, введите корректный российский номер.")]
		[DisplayName("Номер телефона:")]
		public string? Phone { get; set; }

		[JsonIgnore]
		public virtual List<Booking>? Bookings { get; set; }

		[JsonIgnore]
		public virtual List<Review>? Reviews { get; set; }

		public string FullName => Patronymic == null ? $"{LastName} {FirstName}" : $"{LastName} {FirstName} {Patronymic}";
	}
}