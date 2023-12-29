using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CustomerInfo.REST.Validation
{
    public class SSNAttribute : ValidationAttribute
    {

        public SSNAttribute()
        {
            const string defaultErrorMessage = "Invalid format of Swedish SSN";
            ErrorMessage ??= defaultErrorMessage; // Set default error message if not set
        }

        // Validation for Swedish Social Security Number
        public override bool IsValid(object? value)
        {
            const string pattern = @"^(19|20)?(\d{6}[-\s]?\d{4})$";
            return (value == null) ? false : Regex.IsMatch(((string)value), pattern);
        }

    }
}
