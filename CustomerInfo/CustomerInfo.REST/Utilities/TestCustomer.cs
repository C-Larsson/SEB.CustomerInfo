using CustomerInfo.REST.Entities;

namespace CustomerInfo.REST.Utilities
{
    public class TestCustomer : Customer
    {
        public TestCustomer()
        {
            SSN = SSNGenerator.GetSSN();
            Name = NameGenerator.GenerateName();
            Email = Name.Replace(" ", ".").ToLower() + "@test.com";
            PhoneNumber = PhoneNumberGenerator.GeneratePhoneNumber();
        }
    }
}
