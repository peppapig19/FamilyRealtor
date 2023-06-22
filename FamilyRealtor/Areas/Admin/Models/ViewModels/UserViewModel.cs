using System.ComponentModel;
using FamilyRealtor.Models.ValidationAttributes;

namespace FamilyRealtor.Areas.Admin.Models.ViewModels
{
	public class UserViewModel
	{
		public string? Id { get; set; }

		[CustomRequired]
		[DisplayName("Электронная почта:")]
		public string Email { get; set; } = null!;

		[CustomRequired]
		[PasswordPropertyText]
		[DisplayName("Новый пароль:")]
		public string Password { get; set; } = null!;

		[DisplayName("Роли (при наличии):")]
		public IList<string>? Roles { get; set; }
	}
}