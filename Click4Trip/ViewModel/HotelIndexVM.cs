using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class HotelIndexVM
    {
        public List<string> countries { get; set; }

        public List<string> cities { get; set; }

        public string hotelId { get; set; }

        public List<HotelSearchResultsVM> hotels { get; set; } = new List<HotelSearchResultsVM>();

        [Required(ErrorMessage = "Country is required")]
        public string country { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string city { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "Start date: Please enter a valid date")]
        public string sdate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "End date: Please enter a valid date")]
        public string edate { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string code { get; set; }

        public double nights { get; set; }
    }
}