using CustomerInfo.REST.Validation;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.Test.Validation
{
    public class Validation_Tests
    {
        // Testing email address validation
        [Theory]
        [InlineData(null, true)] // null is allowed
        [InlineData("", false)] // empty string is not allowed
        [InlineData(" ", false)] // whitespace is not allowed
        [InlineData("test", false)] // missing @ is not allowed
        [InlineData("test@", false)] // missing domain is not allowd
        [InlineData("test@test", true)] // missing top level domain is allowed
        [InlineData("test@test.com", true)] // top level domain .com
        public void EmailAddressValidation_Tests(string email, bool expectedResult)
        {
            // Arrange
            var customer = new Customer()
            {
                SSN = "197210161234",
                Email = email
            };

            var context = new ValidationContext(customer, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(customer, context, results, true);

            // Assert
            Assert.Equal(expectedResult, isValid);
        }

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

            // Arrange
            var customer = new Customer()
            {
                SSN = ssn,
            };

            var context = new ValidationContext(customer, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(customer, context, results, true);


            Assert.Equal(expectedResult, isValid);
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
            // Arrange
            var customer = new Customer()
            {
                SSN = "197210161234",
                PhoneNumber = phoneNumber
            };

            var context = new ValidationContext(customer, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(customer, context, results, true);
            Assert.Equal(expectedResult, isValid);
        }


    }
}
