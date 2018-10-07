using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class PackageInfoVM
    {
        public HotelOrderDetailsVM hotel { get; set; }
        public FlightInfoVM flight { get; set; }
        public double price { get; set; }
        public double nights { get; set; }
        public string composition { get; set; }
    }
}