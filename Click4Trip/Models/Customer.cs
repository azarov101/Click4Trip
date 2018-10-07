using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class Customer
    {
        [Key]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "ID required")]
        [StringLength(9, ErrorMessage = "ID is 9 charachters")]
        public string ID { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression("^[A-Z a-z]{2,50}$", ErrorMessage = "Full Name should contains only letters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Enter valid Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [RegularExpression("^[A-Z a-z 0-9 ,]{2,50}$", ErrorMessage = "Location should contains only letters")]
        public string Location { get; set; }
    }
}