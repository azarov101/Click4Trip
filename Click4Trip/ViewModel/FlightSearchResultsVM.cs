using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class FlightSearchResultsVM
    {
        public int selectedFlight { get; set; }
        public string departureDate { get; set; }
        public string returnDate { get; set; }
        public string originCountry { get; set; }
        public string originCity { get; set; }
        public string destinationCountry { get; set; }
        public string destinationCity { get; set; }
        public string originCodeCity { get; set; }
        public string destinationCodeCity { get; set; }

        public List<FlightRouteVM> outbound { get; set; } = new List<FlightRouteVM>();
        public List<FlightRouteVM> inbound { get; set; } = new List<FlightRouteVM>();
        public List<string> seatsRemaining { get; set; } = new List<string>();
        public List<string> airline { get; set; } = new List<string>();
        public List<double> price { get; set; } = new List<double>();

        // for google map
        public string apiKey { get; set; } = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

}