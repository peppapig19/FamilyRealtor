using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FamilyRealtor.Models.DomainModels;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Models.ViewModels
{
	public class RegisterViewModel : Client
	{
		[CustomRequired]
		[EmailAddress(ErrorMessage = "Пожалуйста, введите корректный адрес.")]
		[DisplayName("Электронная почта:")]
		public string? Email { get; set; }

		[CustomRequired]
		[DataType(DataType.Password)]
		[Compare(nameof(ConfirmPassword), ErrorMessage = "Пароль и подтверждение не совпадают.")]
		[DisplayName("Пароль:")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Пожалуйста, подтвердите пароль.")]
		[PasswordPropertyText]
		[DisplayName("Подтверждение пароля:")]
		public string? ConfirmPassword { get; set; }
	}
}