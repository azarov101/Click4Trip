using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarRentSearchResultsVM
    {
        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }
        public string LocationCodeCity { get; set; }

        public List<string> provider { get; set; } = new List<string>();
        public List<string> branch_id { get; set; } = new List<string>();
        public List<string> airport { get; set; } = new List<string>();
        public List<string> airportName { get; set; } = new List<string>();
        public List<string> address { get; set; } = new List<string>();
        public List<CarRentVM> cars { get; set; } = new List<CarRentVM>();

        public List<double> latitude { get; set; } = new List<double>();
        public List<double> longitude { get; set; } = new List<double>();
    }
}