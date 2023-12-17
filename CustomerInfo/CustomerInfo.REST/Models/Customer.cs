using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.Models
{
    public class Customer
    {
        [Required(ErrorMessage = "SocialSecurityNumber is required")]
        public string SSN { get; set; }

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
