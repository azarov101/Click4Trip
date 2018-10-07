using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class ViewHotelOrderVM
    {
        public int invoice { get; set; }
        public string customerName { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string orderDate { get; set; }
        public string creditCardNumber { get; set; }
        public int status { get; set; }
        public string hotelName { get; set; }
        //public string hotelAddress { get; set; }
        public List<RoomVM> rooms { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public double totalPrice { get; set; }
        public int numberOfRooms { get; set; }

    }
}