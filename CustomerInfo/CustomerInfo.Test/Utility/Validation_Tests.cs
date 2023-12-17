using CustomerInfo.REST.Utility;

namespace CustomerInfo.Test.Utility
{
    public class Validation_Tests
    {
        [InlineData(null, false)] // null is not allowed
        [InlineData("197210161234", true)] // 12 digits
        [InlineData("7210163123", true)] // 10 digits
        [InlineData("19721016-1234", true)] // 12 digits with dash
        [InlineData("721016-1234", true)] // 10 digits with dash
        [InlineData("19721016123", false)] // 11 digits
        [InlineData("97210161234", false)] // 11 digits
        [InlineData("19721016-123", false)] // 11 digits with dash
        [InlineData("9721016-1234", false)] // 11 digits with dash
        [Theory]
        public void SsnValidation_Tests(string ssn, bool expectedResult)
        {
            var actualResult = SsnValidator.IsValid(ssn);
            Assert.Equal(expectedResult, actualResult);
        }

        // Testing phone number validation
        [Theory]
        [InlineData("0701234567", true)] // 10 digits
        [InlineData("+46701234567", true)] // 12 digits
        [InlineData("070123456", false)] // 9 digits
        [InlineData("07012345678", false)] // 11 digits
        [InlineData("070-123456", false)] // 9 digits with dash
        [InlineData("070-12345678", false)] // 11 digits with dash
        public void PhoneNumberValidation_Tests(string phoneNumber, bool expectedResult)
        {
            var actualResult = PhoneNumberValidator.IsValid(phoneNumber);
            Assert.Equal(expectedResult, actualResult);
        }


    }
}
