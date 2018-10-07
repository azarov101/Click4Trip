using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class PackageCheckoutVM
    {
        public string hotelName { get; set; }
        public int numberOfRooms { get; set; }
        public string roomDescription { get; set; }
        public string composition { get; set; }


        public string departureFlightNumber { get; set; }
        public string returnFlightNumber { get; set; }
        public string departureDate { get; set; }
        public string returnDate { get; set; }
        public string departureLocation { get; set; }
        public string returnLocation { get; set; }
        public int numberOfTickets { get; set; }
        public string airline { get; set; }
        public string totalFee { get; set; }
        public string destCountry { get; set; }


        public string customerName { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string location { get; set; }
        public string phone { get; set; }
        public string cardName { get; set; }
        public string id { get; set; }
        public string creditCard { get; set; }
        public string cardExpDate { get; set; }
        public string cardCvv { get; set; }

    }
}