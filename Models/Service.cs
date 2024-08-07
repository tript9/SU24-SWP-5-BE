﻿using System.ComponentModel.DataAnnotations;

namespace SWPApp.Models
{
    public class Service
    {
        [Key]
        public string ServiceId { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public int ServicePrice { get; set; } 
        public int Duration { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
