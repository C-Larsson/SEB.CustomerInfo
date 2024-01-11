using CustomerInfo.REST.Validation;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.Models
{
    public class Customer
    {
        [Key]
        [SSN]
        public string SSN { get; set; }

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        public string? Email { get; set; }

        [SwePhone]
        public string? PhoneNumber { get; set; }

    }
}
