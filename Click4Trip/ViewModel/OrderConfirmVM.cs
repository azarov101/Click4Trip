using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class OrderConfirmVM
    {
        public List<ViewHotelOrderVM> hotelOrderVM { get; set; } = new List<ViewHotelOrderVM>();
        public List<ViewFlightOrderVM> flightOrderVM { get; set; } = new List<ViewFlightOrderVM>();
        public List<ViewPackageOrderVM> packageOrderVM { get; set; } = new List<ViewPackageOrderVM>();
        public List<ViewCarRentVM> carRentOrderVM { get; set; } = new List<ViewCarRentVM>();
    }
}