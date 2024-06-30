using System;
using System.ComponentModel.DataAnnotations;

namespace SWPApp.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int CustomerId { get; set; }

        public int? EmployeeId { get; set; } // Nullable property for EmployeeId

        public DateTime? RequestDate { get; set; }

        public string? Email { get; set; } // Nullable Email property

        public string? PhoneNumber { get; set; }

        public string? IDCard { get; set; }

        public string? Address { get; set; }

        public string ?ServiceId { get; set; }

        public string?Status { get; set; }

        public Customer Customer { get; set; }

        public Employee Employee { get; set; } // Navigation property for Employee
    }
}
