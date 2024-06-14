using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWPApp.Models
{
    // Models/Employee.cs
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }
        public int Role { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        public string? LoginToken { get; set; }
        [NotMapped]
        public DateTime? LoginTokenExpires { get; set; }
    }

}
