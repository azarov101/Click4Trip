using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class SupportTicket
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please describe your issue")]
        public string Note { get; set; }

        [RegularExpression("^[0-9]{9,10}$", ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression("^[A-Z a-z]{2,50}$", ErrorMessage = "Full Name should contains only letters")]
        public string FullName { get; set; }

        public int Resolved { get; set; }
    }
}