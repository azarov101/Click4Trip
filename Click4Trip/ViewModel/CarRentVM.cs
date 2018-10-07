using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarRentVM
    {
        //vehicle_info
        public string acriss_code { get; set; }
        public string transmission { get; set; }
        public string fuel { get; set; }
        public bool air_conditioning { get; set; }
        public string category { get; set; }
        public string car_type { get; set; }
        //rates
        public string rate_type { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        //images
        public string image_category { get; set; }
        public int image_width { get; set; }
        public int image_height { get; set; }
        public string image_url { get; set; }
        //estimated_total
        public double estimated_total_amount { get; set; }
        public string estimated_total_currency { get; set; }
    }
}