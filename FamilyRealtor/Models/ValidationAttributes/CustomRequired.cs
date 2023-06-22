using System.ComponentModel.DataAnnotations;

namespace FamilyRealtor.Models.ValidationAttributes
{
	public class CustomRequired : RequiredAttribute
	{
		public CustomRequired()
		{
			ErrorMessage ??= "Пожалуйста, заполните поле.";
		}
	}
}