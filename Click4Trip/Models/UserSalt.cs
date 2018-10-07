using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class UserSalt
    {
        [Key]
        public string Email { get; set; }

        public string Salt { get; set; }
    }
}