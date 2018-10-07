using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class HotelCheckoutVM
    {
        public string hotelName { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public int numberOfRooms { get; set; }

        public List<RoomVM> room { get; set; }

        public string customerName { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string location { get; set; }

        //[RegularExpression(@"^+[0-9]{3}-[0-9]{9}*$", ErrorMessage = "Not a valid phone number")]
        public string phone { get; set; }

        public string cardName { get; set; }
        public string id { get; set; }
        public string creditCard { get; set; }
        public string cardExpDate { get; set; }
        public string cardCvv { get; set; }

        public string totalFee { get; set; }
        public string hotelAddress { get; set; }

        public string destination { get; set; }
    }
}