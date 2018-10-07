using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class EmailsVM
    {
        public List<string> emails { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        public string selectedEmail { get; set; }

        [Required(ErrorMessage = "Password required")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password between 8-12 charachters")]
        public string password { get; set; }
    }
}