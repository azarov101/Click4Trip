using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarRent
    {
        public string provider { get; set; }
        public string branch_id { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public CarRentVM car { get; set; }
    }
}