using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProffyBackend.Models.PropertyValidators
{
    public class LocaleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return null;
            try
            {
                var parsedValue = value.ToString();
                if (parsedValue != null)
                {
                    CultureInfo.GetCultureInfo(parsedValue);
                    return null;
                }
            }
            catch (CultureNotFoundException)
            {
            }

            return new ValidationResult("Invalid locale");
        }
    }
}