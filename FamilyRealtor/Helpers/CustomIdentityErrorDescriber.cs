using Microsoft.AspNetCore.Identity;

namespace FamilyRealtor.Helpers
{
	public class CustomIdentityErrorDescriber : IdentityErrorDescriber
	{
		public override IdentityError PasswordMismatch()
		{
			return new IdentityError
			{
				Code = nameof(PasswordMismatch),
				Description = "Неправильный старый пароль."
			};
		}

		public override IdentityError DuplicateEmail(string email)
		{
			return new IdentityError
			{
				Code = nameof(DuplicateEmail),
				Description = "Пользователь с данным адресом уже существует."
			};
		}

		public override IdentityError PasswordRequiresDigit()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresDigit),
				Description = "Пароль должен содержать цифру."
			};
		}

		public override IdentityError PasswordTooShort(int length)
		{
			return new IdentityError
			{
				Code = nameof(PasswordTooShort),
				Description = "Пароль должен содержать не менее 6 символов."
			};
		}

		public override IdentityError PasswordRequiresLower()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresLower),
				Description = "Пароль должен содержать символ в нижнем регистре."
			};
		}

		public override IdentityError PasswordRequiresUpper()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresUpper),
				Description = "Пароль должен содержать символ в верхнем регистре."
			};
		}
	}
}