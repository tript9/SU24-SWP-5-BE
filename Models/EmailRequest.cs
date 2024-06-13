using System.ComponentModel.DataAnnotations;

namespace SWPApp.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }

    public class ConfirmationEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ConfirmationToken { get; set; }
    }

    public class ResetPasswordEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ResetToken { get; set; }
    }
}
