using CustomerInfo.REST.Validation;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.DTOs
{
    public class CustomerDto
    {
        [SSN]
        public required string SSN { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        public string? Email { get; set; }

        [SwePhone]
        public string? PhoneNumber { get; set; }


    }
}
