using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWPApp.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RequestDate { get; set; }
        public string ServiceType { get; set; }
        public int EmployeeId { get; set; }
        public int? DiamondId { get; set; }
        public string ServiceId { get; set; }
        public bool Status { get; set; }

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Diamond Diamond { get; set; }
    }

    public class RequestDetail
    {
        [Key]
        public int RequestId { get; set; }
        public string ServiceId { get; set; }
        public bool PaymentStatus { get; set; } // Changed from string to bool
        public int PaymentMethod { get; set; }

        public Request Request { get; set; }
        public Service Service { get; set; }
    }
}
