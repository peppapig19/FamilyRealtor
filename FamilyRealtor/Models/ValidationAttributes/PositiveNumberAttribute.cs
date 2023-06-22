using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamilyRealtor.Models.ValidationAttributes
{
	public class PositiveNumberAttribute : RegularExpressionAttribute
	{
		public PositiveNumberAttribute([StringSyntax("Regex")] string pattern = @"^\d*$") : base(pattern)
		{
			ErrorMessage ??= "Пожалуйста, введите корректное число.";
		}
	}
}