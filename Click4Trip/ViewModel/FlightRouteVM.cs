using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class FlightRouteVM
    {
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureAirportCode { get; set; }
        public string ArrivalAirportCode { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirportName { get; set; }
        public string ArrivalAirportName { get; set; }
        public string DepartureDay { get; set; }
        public string ArrivalDay { get; set; }

        public string TimeDuration { get; set; }


    }
}