using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FamilyRealtor.Models.ValidationAttributes
{
    public class AgeRangeAttribute : ValidationAttribute, IClientModelValidator
    {
        readonly int _min = 18;
        readonly int _max = 100;

        protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
        {
            if (value != null && DateOnly.TryParse(value.ToString(), out var dateOnly))
			{
				DateTime DOB = new(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0);
				DateTime olderThan = DateTime.Now.AddYears(-_min);
				DateTime youngerThan = DateTime.Now.AddYears(-_max);

                if (DOB < olderThan && DOB > youngerThan) return ValidationResult.Success;
            }
            else return ValidationResult.Success;

			string msg = ErrorMessage ?? "Введённый возраст недопустим.";
            return new ValidationResult(msg);
        }

        public void AddValidation(ClientModelValidationContext ctx)
        {
            string msg = ErrorMessage ?? "Введённый возраст недопустим.";

            if (!ctx.Attributes.ContainsKey("data-val")) ctx.Attributes.Add("data-val", "true");
            if (!ctx.Attributes.ContainsKey("data-val-ageRange")) ctx.Attributes.Add("data-val-ageRange", msg);
        }
    }
}