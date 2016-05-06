using System.Globalization;
using System.Windows.Controls;

namespace CoilSimulater.Utilities.ValidationRules
{
    public class NumberInputValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double number;
            if (!double.TryParse(value.ToString(), out number))
            {
                return new ValidationResult(false, "Input should be a number.");
            }

            return new ValidationResult(true, string.Empty);
        }
    }
}
