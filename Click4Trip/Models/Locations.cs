using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class Locations
    {
        [Key]
        public string code { get; set; }

        public string country { get; set; }

        public string city { get; set; }
    }
}