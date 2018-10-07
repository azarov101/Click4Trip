using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class PackageDetails
    {
        //Hotel:
        public HotelSearchResultsVM hotel { get; set; }

        //Flight:
        public FlightInfoVM flight { get; set; }

        public double price { get; set; }
    }
}