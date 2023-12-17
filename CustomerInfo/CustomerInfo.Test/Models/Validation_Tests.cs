using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.Test.Models
{
    public class Validation_Tests
    {
        // Testing email address validation
        [Theory]
        [InlineData(null, true)] // null is allowed
        [InlineData("", false)] // empty string is not allowed
        [InlineData(" ", false)] // whitespace is not allowed
        [InlineData("test", false)] // no @
        [InlineData("test@", false)] // no domain
        [InlineData("test@test.com", true)] // top level domain .com
        public void EmailAddressValidation_Tests(string email, bool expectedResult)
        {
            // Arrange
            var customerInfo = new REST.Models.CustomerInfo()
            {
                SocialSecurityNumber = "197210161234",
                Email = email
            };

            var context = new ValidationContext(customerInfo, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(customerInfo, context, results, true);

            // Assert
            Assert.Equal(expectedResult, isValid);
        }
    }
}
