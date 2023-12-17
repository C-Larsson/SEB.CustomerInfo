using System.Text.RegularExpressions;

namespace CustomerInfo.REST.Utility
{
    public static class SsnValidator
    {
        // Validation for Swedish Social Security Number
        public static bool IsValid(string ssn)
        {
            if (ssn == null) 
                return false;

            string pattern = @"^(19|20)?(\d{6}[-\s]?\d{4})$";
            return Regex.IsMatch(ssn, pattern);
        }
    }
}
