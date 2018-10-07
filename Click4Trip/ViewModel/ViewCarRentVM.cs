using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class ViewCarRentVM
    {
        public int invoice { get; set; }
        public string customerName { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string orderDate { get; set; }
        public string creditCardNumber { get; set; }
        public int status { get; set; }

        public string PickUpDate { get; set; }
        public string DropOffDate { get; set; }
        public int days { get; set; }
        public string address { get; set; }
        public string provider { get; set; }
        public string acrisscode { get; set; }
        public double Price { get; set; }
        public double totalPrice { get; set; }
    }
}