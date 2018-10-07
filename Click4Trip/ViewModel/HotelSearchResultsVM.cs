using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class HotelSearchResultsVM
    {
        [Required(ErrorMessage = "hotelLink is required")]
        public string hotelLink { get; set; }

        public string originalLink { get; set; } // ment to fix the api room problem (get at least 1 room)


        [Required(ErrorMessage = "hotelName is required")]
        public string hotelName { get; set; }

        [Required(ErrorMessage = "raiting is required")]
        public int rating { get; set; }

        [Required(ErrorMessage = "hotelDescription is required")]
        public string hotelDescription { get; set; }

        [Required(ErrorMessage = "hotelPrice is required")]
        public double hotelPrice { get; set; }

        [Required(ErrorMessage = "hotelPrice is required")]
        public string hotelImage { get; set; }

        public string location { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }
    }
}