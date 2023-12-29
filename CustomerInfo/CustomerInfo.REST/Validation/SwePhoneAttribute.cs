using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CustomerInfo.REST.Validation
{
    public class SwePhoneAttribute : ValidationAttribute
    {

        public SwePhoneAttribute()
        {
            const string defaultErrorMessage = "Invalid format of Swedish phone number";
            ErrorMessage ??= defaultErrorMessage; // Set default error message if not set
        }

        public override bool IsValid(object? value)
        {
            const string pattern = @"^(\+46|0)[\d]{9}$";
            return (value == null) ? true : Regex.IsMatch(((string)value), pattern);
        }

    }
}
