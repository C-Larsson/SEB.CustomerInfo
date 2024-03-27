using CustomerInfo.REST.Validation;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.Entities
{
    public class Customer : BaseEntity
    {
        [Key]
        public int Id { get; set; }

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
