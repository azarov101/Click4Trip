using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class FlightInfoVM
    {
        public string departureDate { get; set; }
        public string returnDate { get; set; }
        public string originCountry { get; set; }
        public string originCity { get; set; }
        public string destinationCountry { get; set; }
        public string destinationCity { get; set; }
        public string originCodeCity { get; set; }
        public string destinationCodeCity { get; set; }
        public string seatsRemaining { get; set; }
        public string airline { get; set; }
        public string price { get; set; }
        public string timeDuration_outbound { get; set; }
        public string timeDuration_inbound { get; set; }
        public FlightRouteVM outbound { get; set; }
        public FlightRouteVM inbound { get; set; }
        public string composition { get; set; }
        public string totalPrice { get; set; }
    }
}