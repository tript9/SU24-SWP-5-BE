using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWPApp.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; } // Nullable string for PhoneNumber

        public string? IDCard { get; set; } // Nullable string for IDCard

        public string? Address { get; set; } // Nullable string for Address

        public bool Status { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpires { get; set; }

        public string? LoginToken { get; set; }

        public DateTime? LoginTokenExpires { get; set; }

        public bool EmailConfirmed { get; set; }

        public string? ConfirmationToken { get; set; } // Make ConfirmationToken nullable

        public DateTime? ConfirmationTokenExpires { get; set; }
    }
}
