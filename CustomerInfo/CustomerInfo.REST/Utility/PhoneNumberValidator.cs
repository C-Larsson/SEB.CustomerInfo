using System.Text.RegularExpressions;

namespace CustomerInfo.REST.Utility
{
    public static class PhoneNumberValidator
    {
        // Validation for a Swedish phone number
        public static bool IsValid(string? phoneNumber)
        {
            if (phoneNumber == null) 
                return true;

            string pattern = @"^(\+46|0)[\d]{9}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
