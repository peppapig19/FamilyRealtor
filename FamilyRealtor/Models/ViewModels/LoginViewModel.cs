using System.ComponentModel;

namespace FamilyRealtor.Models.ViewModels
{
	public class LoginViewModel
	{
		[DisplayName("Электронная почта:")]
		public string Email { get; set; } = null!;

		[PasswordPropertyText]
		[DisplayName("Пароль:")]
		public string Password { get; set; } = null!;

		[DisplayName("Запомнить?")]
		public bool RememberMe { get; set; }

		public string? ReturnUrl { get; set; }
	}
}