namespace CustomerInfo.REST.Utilities
{
    public static class PhoneNumberGenerator
    {

        // Generate a random Swedish phone number
        public static string GeneratePhoneNumber()
        {
            Random random = new Random();
            string[] prefixes = { "+4670", "+4672", "+4673", "+4676", "+4679" };
            string phoneNumber = prefixes[random.Next(0, prefixes.Length)] + random.Next(1000000, 9999999).ToString();
            return phoneNumber;
        }


    }
}
