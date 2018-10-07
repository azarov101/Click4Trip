using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CitiesVM
    {
        public List<string> cities { get; set; }

        [Required(ErrorMessage = "City name is required")]
        public string selectedCity { get; set; }
    }
}