using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.ViewModels
{
	public class AccountViewModel
	{
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
		[RegularExpression(@"^(\+7\d{10})$", ErrorMessage = "Пожалуйста, введите корректный российский номер.")]
		[DisplayName("Номер телефона:")]
		public string? Phone { get; set; }

		[PasswordPropertyText]
		[DisplayName("Старый пароль:")]
		public string? OldPassword { get; set; }

		[DataType(DataType.Password)]
		[Compare(nameof(ConfirmPassword), ErrorMessage = "Пароль и подтверждение не совпадают.")]
		[DisplayName("Новый пароль:")]
		public string? NewPassword { get; set; }

		[PasswordPropertyText]
		[DisplayName("Подтверждение пароля:")]
		public string? ConfirmPassword { get; set; }
	}
}