namespace SWPApp.DTO
{
  
        public class RequestDTO
        {
            public int CustomerId { get; set; }
            public DateTime RequestDate { get; set; }
            public string ServiceType { get; set; }
            public int EmployeeId { get; set; }
            public int? DiamondId { get; set; }
            public string ServiceId { get; set; }
            public bool Status { get; set; }
        }
    }

