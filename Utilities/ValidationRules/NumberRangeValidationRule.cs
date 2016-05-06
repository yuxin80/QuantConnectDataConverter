using System;
using System.Globalization;
using System.Windows.Controls;

namespace CoilSimulater.Utilities.ValidationRules
{
    public class NumberRangeValidationRule : ValidationRule
    {
        public double Min { get; set; }

        public double Max { get; set; }

        public NumberRangeValidationRule()
            : base()
        {
            Min = double.MinValue;
            Max = double.MaxValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var numberInputVal = new NumberInputValidationRule();
            var result = numberInputVal.Validate(value, cultureInfo);
            if (!result.IsValid)
                return result;

            double v = Convert.ToDouble(value);

            if (v < Min || v > Max)
            {
                result = new ValidationResult(false, string.Format("Value should be in the range of [{0},{1}]", Min, Max));
                return result;
            }

            return new ValidationResult(true, string.Empty);
        }
    }
}
