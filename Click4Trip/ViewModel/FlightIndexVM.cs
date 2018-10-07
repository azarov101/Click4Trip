using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class FlightIndexVM
    {
        public List<string> originCountries { get; set; }

        public List<string> originCities { get; set; }

        //public string hotelId { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string originCountry { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string originCity { get; set; }

        public List<string> desCountries { get; set; }

        public List<string> desCities { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string desCountry { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string desCity { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "Start date: Please enter a valid date")]
        public string sdate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "End date: Please enter a valid date")]
        public string edate { get; set; }

        public string originCode { get; set; }

        public string desCode { get; set; }
    }
}