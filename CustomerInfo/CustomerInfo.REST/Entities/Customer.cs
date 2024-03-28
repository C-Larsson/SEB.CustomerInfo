using CustomerInfo.REST.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.REST.Entities
{
    [Index(nameof(SSN), IsUnique = true)]
    public class Customer : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [SSN, Required]
        public string SSN { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        public string? Email { get; set; }

        [SwePhone]
        public string? PhoneNumber { get; set; }

    }
}
