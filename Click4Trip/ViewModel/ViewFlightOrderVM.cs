using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class ViewFlightOrderVM
    {
        public int invoice { get; set; }
        public string customerName { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string orderDate { get; set; }
        public string creditCardNumber { get; set; }
        public int status { get; set; }
        public int outbound { get; set; }
        public int inbound { get; set; }
        public string DetartureLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public string DetartureDate { get; set; }
        public string ReturnDate { get; set; }
        public double totalPrice { get; set; }
        public int NumBabies { get; set; }
        public int NumChild { get; set; }
        public int NumYoung { get; set; }
        public int NumAdult { get; set; }
        public int NumPensioner { get; set; }
        public double PriceBabies { get; set; }
        public double PriceChild { get; set; }
        public double PriceYoung { get; set; }
        public double PriceAdult { get; set; }
        public double PricePensioner { get; set; }
        public string Airline { get; set; }
    }
}