using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FamilyRealtor.Models.ValidationAttributes
{
	public class GreaterThanAttribute : ValidationAttribute, IClientModelValidator
	{
		readonly string _comparisonProperty;

		public GreaterThanAttribute(string comparisonProperty)
		{
			_comparisonProperty = comparisonProperty;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
		{
			PropertyInfo? property = ctx.ObjectType.GetProperty(_comparisonProperty);
			object? propertyValue = property?.GetValue(ctx.ObjectInstance);

			if (value == null || propertyValue == null || ReferenceEquals(value.GetType(), propertyValue.GetType())
				&& value is IComparable currentValue && propertyValue is IComparable comparisonValue
				&& currentValue.CompareTo(comparisonValue) != -1) return ValidationResult.Success;

			string msg = ErrorMessage ?? "Значение меньше предыдущего.";
			return new ValidationResult(msg);
		}

		public void AddValidation(ClientModelValidationContext ctx)
		{
			string msg = ErrorMessage ?? "Значение меньше предыдущего.";

			if (!ctx.Attributes.ContainsKey("data-val")) ctx.Attributes.Add("data-val", "true");
			if (!ctx.Attributes.ContainsKey("data-val-greaterThan")) ctx.Attributes.Add("data-val-greaterThan", msg);

			if (!ctx.Attributes.ContainsKey("data-val-greaterThan-property"))
				ctx.Attributes.Add("data-val-greaterThan-property", _comparisonProperty);
		}
	}
}