using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.Models
{
    public class CustomerInfo
    {
        [Required(ErrorMessage = "SocialSecurityNumber is required")]
        public string SocialSecurityNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
