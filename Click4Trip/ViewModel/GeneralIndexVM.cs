using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class GeneralIndexVM
    {
        public List<string> countries { get; set; }
        public List<string> cities { get; set; }


        public string desCountry { get; set; }
        public string desCity { get; set; }
        public string originCountry { get; set; }
        public string originCity { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }

        public List<string> compositions { get; set; } = new List<string>()
        {"Couple","Single","Adult + Child","Adult + 2 Children","Adult + 3 Children","Couple + Child","Couple + 2 Children","Couple + 3 Children","3 Adults" };
        public string composition { get; set; }


        public string hotelId { get; set; }
        public string hotelRating { get; set; }
        public string originalLink { get; set; }
        public double nights { get; set; }
    }
}