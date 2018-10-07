using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarRentCheckoutVM
    {
        public string pickUpDate { get; set; }
        public string dropOffDate { get; set; }
        public string destCountry { get; set; }
        //public string destCity { get; set; }
        public double days { get; set; }
        public string address { get; set; }
        public string provider { get; set; }
        public double price { get; set; }
        public string acriss_code { get; set; }

        public string customerName { get; set; }
        public string email { get; set; }
        public string customerAddress { get; set; }
        public string phone { get; set; }
        public string cardName { get; set; }
        public string id { get; set; }
        public string creditCard { get; set; }
        public string cardExpDate { get; set; }
        public string cardCvv { get; set; }

        public CarRentCheckoutVM(CarInfoVM obj)
        {
            this.pickUpDate = obj.pickUpDate;
            this.dropOffDate = obj.dropOffDate;
            this.destCountry = obj.destCountry;
            this.days = obj.days;
            this.address = obj.car.address;
            this.provider = obj.car.provider;
            this.price = obj.car.car.estimated_total_amount;
            this.acriss_code = obj.car.car.acriss_code;
        }

        public CarRentCheckoutVM() { } // do not delete!
    }
}