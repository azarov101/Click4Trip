using Click4Trip.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class HotelOrderDetailsVM
    {
        public string hotelName { get; set; }

        public string rating { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string address { get; set; }

        public List<string> contact { get; set; }

        public List<string> amenities { get; set; }

        public List<RoomVM> room { get; set; }

        public List<string> image { get; set; }

        public SearchResultsVM searchResultsVM { get; set; }

        public List<HotelReview> reviews { get; set; }
        public List<string> customersName { get; set; }


        // need only for GOOGLE MAP
        public string apiKey { get; set; }

        public string destination { get; set; }

        // those property only for payment page
        public int numberOfRooms { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public string roomComposition1 { get; set; }
        public string roomComposition2 { get; set; }
        public string roomComposition3 { get; set; }

        public string roomType1 { get; set; }
        public string roomType2 { get; set; }
        public string roomType3 { get; set; }

        public double roomPrice1 { get; set; }
        public double roomPrice2 { get; set; }
        public double roomPrice3 { get; set; }
    }
}