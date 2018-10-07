using Click4Trip.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class CarRentViewVM
    {
        public string PickupDate { get; set; }
        public string DropoffDate { get; set; }
        public string LocationCodeCity { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int selectedCar { get; set; }
        public double days { get; set; }

        // for google map
        public string apiKey { get; set; } = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
        public double latitude { get; set; }
        public double longitude { get; set; }

        public List<CarRent> results { get; set; }

        public CarRentViewVM() { }

        public CarRentViewVM(CarRentSearchResultsVM obj)
        {
            PickupDate = obj.PickupDate;
            DropoffDate = obj.DropoffDate;
            LocationCodeCity = obj.LocationCodeCity;
            selectedCar = -1;

            days = ToolsClass.getNumOfNights(ToolsClass.getDate(PickupDate), ToolsClass.getDate(DropoffDate)); // +1; do not touch (mike)

            results = new List<CarRent>();

            foreach (CarRentVM car in obj.cars)
            {
                CarRent c = new CarRent();
                var rand = new Random();
                int n = rand.Next(obj.provider.Count());
                c.address = obj.address[n];
                c.branch_id = obj.branch_id[n];
                c.provider = obj.provider[n];
                c.latitude = obj.latitude[n];
                c.longitude = obj.longitude[n];
                c.car = car;
                results.Add(c);
            }
        }

    }
}