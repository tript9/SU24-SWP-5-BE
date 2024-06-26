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
        [EmailAddress]
        public string Email { get; set; }
     
        public string? PhoneNumber { get; set; } // Nullable string for PhoneNumber

        public string? IDCard { get; set; } // Nullable string for IDCard

        public string? Address { get; set; } // Nullable string for Address
        public string ServiceType { get; set; }

        public string ServiceId { get; set; }
        public string Status { get; set; }

        public Customer Customer { get; set; }

       
    }




    public class RequestDetail
    {
        [Key]
        public int RequestId { get; set; }
        public string ServiceId { get; set; }
        public bool PaymentStatus { get; set; } 
        public int PaymentMethod { get; set; }

        public Request Request { get; set; }
        public Service Service { get; set; }
    }
   
}
