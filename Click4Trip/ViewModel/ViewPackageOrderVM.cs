using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class ViewPackageOrderVM
    {
        public int invoice { get; set; }
        public string customerName { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string orderDate { get; set; }
        public string creditCardNumber { get; set; }
        public double totalPrice { get; set; }
        public string DetartureLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public string DetartureDate { get; set; }
        public string ReturnDate { get; set; }
        public int outbound { get; set; }
        public int inbound { get; set; }
        public string Airline { get; set; }
        public int numTickets { get; set; }
        public string hotelName { get; set; }
        public int numberOfRooms { get; set; }
        public string composition { get; set; }
        public string roomdescription { get; set; }
        public int status { get; set; }
    }
}