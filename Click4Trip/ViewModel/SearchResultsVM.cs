using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class SearchResultsVM
    {
        public List<HotelSearchResultsVM> hotelSearchResultsVM { get; set; }

        public string startdDate { get; set; }

        public string endDate { get; set; }

        public double nights { get; set; }

        public string hotelId { get; set; }
        public string hotelRating { get; set; }

        public string originalLink { get; set; } // ment to fix the api room problem (get at least 1 room)

        public int selectedHotel { get; set; } // the number of the hotel from the whole hotels list (need for the api fix hack)

        public string destination { get; set; } // need for the data mining

        // for google map
        public string apiKey { get; set; } = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}