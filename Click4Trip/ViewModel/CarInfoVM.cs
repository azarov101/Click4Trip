using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarInfoVM
    {
        public string pickUpDate { get; set; }
        public string dropOffDate { get; set; }
        public string destCountry { get; set; }
        public string destCity { get; set; }
        public double days { get; set; }
        public CarRent car { get; set; }
        public string apiKey { get; set; }

        public CarInfoVM(CarRentViewVM obj)
        {
            this.apiKey = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
            this.pickUpDate = obj.PickupDate;
            this.dropOffDate = obj.DropoffDate;
            this.destCountry = obj.Country;
            this.destCity = obj.City;
            this.days = obj.days;
            this.car = obj.results[obj.selectedCar];
        }

        public CarInfoVM()
        {
            this.apiKey = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
            this.pickUpDate = "15.05.2018";
            this.dropOffDate = "16.05.2018";
            this.destCountry = "Israel";
            this.destCity = "Tel Aviv";
            this.days = 2;
            this.car = new CarRent()
            {
                address = "BEN GURION AIRPORT, TEL AVIV, IL",
                branch_id = "TLVT01",
                provider = "ZI, AVIS",
                latitude = 31.98333,
                longitude = 34.88333,
                car = new CarRentVM()
                {
                    acriss_code = "ECMR",
                    air_conditioning = true,
                    amount = 29,
                    car_type = "2/4 Door",
                    category = "Economy",
                    currency = "USD",
                    estimated_total_amount = 97.5,
                    estimated_total_currency = "USD",
                    fuel = "Unspecified",
                    image_category = "VEHICLE",
                    image_height = 50,
                    image_url = "https://multimedia.amadeus.com/mdc/retrieveCarItem?ctg=VEHICLE&prov=ZD&cnt=IL&vehcat=ECMR&item=0&stamp=VEHICLE_0__0__1360257483279&file=1.JPEG",
                    image_width = 90,
                    rate_type = "DAILY",
                    transmission = "Manual",
                }
            };
        }
    }
}