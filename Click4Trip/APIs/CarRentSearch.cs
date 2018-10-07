using Click4Trip.Tools;
using Click4Trip.ViewModel;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Click4Trip.APIs
{
    public class CarRentSearch
    {
        // general fields
        private string PickupDate;
        private string DropoffDate;
        private string LocationCodeCity;


        private List<string> provider;
        private List<string> branch_id;
        private List<string> airport;
        private List<string> address;
        private List<double> latitude;
        private List<double> longitude;
        private List<CarRentVM> cars;

        // class helper fields
        private string apikey;
        private string searchURL;
        private string json;
        private string response; // to know if there was an error with the request

        public CarRentSearch()
        {
            apikey = "FsvHjlWHVR56hqdQ46ibM2M6JRpbM8EZ"; // michael new api 28/05

            //departure_date = new List<string>();
            //return_date = new List<string>();
            provider = new List<string>();
            branch_id = new List<string>();
            airport = new List<string>();
            latitude = new List<double>();
            longitude = new List<double>();
            address = new List<string>();
            cars = new List<CarRentVM>();

            response = "";
        }

        public void FillData(string locationCode, string startDate, string endDate)
        {
            this.PickupDate = AdjustDateToAPI(startDate);
            this.DropoffDate = AdjustDateToAPI(endDate);
            this.LocationCodeCity = locationCode;

            BuildURL();
            GetJson();

            if (response != "") // there was an error with the request
                return;

            dynamic results = JsonConvert.DeserializeObject(json);

            for (int i = 0; i < results.results.Count; ++i)
            {
                try
                {
                    latitude.Add(Convert.ToDouble(results.results[i].location.latitude.ToString()));
                    longitude.Add(Convert.ToDouble(results.results[i].location.longitude.ToString()));
                }
                catch (RuntimeBinderException)
                {
                    continue; // if there is no location information - i dont need that shit
                }
                for(int j=0; j < results.results[i].cars.Count; j++)
                {
                    CarRentVM temp_cars = new CarRentVM();

                    temp_cars.acriss_code = results.results[i].cars[j].vehicle_info.acriss_code;
                    temp_cars.transmission = results.results[i].cars[j].vehicle_info.transmission;
                    temp_cars.fuel = results.results[i].cars[j].vehicle_info.fuel;
                    if (results.results[i].cars[j].vehicle_info.air_conditioning != null)
                        temp_cars.air_conditioning = results.results[i].cars[j].vehicle_info.air_conditioning;//check later
                    else
                        temp_cars.air_conditioning = false;

                    temp_cars.category = results.results[i].cars[j].vehicle_info.category;
                    temp_cars.car_type = results.results[i].cars[j].vehicle_info.type;

                    temp_cars.rate_type = results.results[i].cars[j].rates[0].type;//USD only
                    temp_cars.amount = results.results[i].cars[j].rates[0].price.amount;//USD only
                    temp_cars.currency = results.results[i].cars[j].rates[0].price.currency;//USD only

                    if (results.results[i].cars[j].images != null)
                    {
                        temp_cars.image_category = results.results[i].cars[j].images[0].category;
                        temp_cars.image_width = results.results[i].cars[j].images[0].width;
                        temp_cars.image_height = results.results[i].cars[j].images[0].height;
                        temp_cars.image_url = results.results[i].cars[j].images[0].url;
                    }
                    else
                    {
                        temp_cars.image_category = "ERROR";
                        temp_cars.image_width = 256;
                        temp_cars.image_height = 256;
                        temp_cars.image_url = "https://www.designontextile.com/js/awant/design/core_img/no_image_available.256.png";
                    }

                    temp_cars.estimated_total_amount = results.results[i].cars[j].estimated_total.amount;
                    temp_cars.estimated_total_currency = results.results[i].cars[j].estimated_total.currency;

                    cars.Add(temp_cars);
                }

                if(results.results[i].provider.company_code == null || results.results[i].provider.company_name == null)
                {
                    provider.Add("No information available.");
                }
                else
                {
                    provider.Add(results.results[i].provider.company_code.ToString() + ", " + results.results[i].provider.company_name.ToString());
                }
                if (results.results[i].branch_id == null)
                {
                    branch_id.Add("No information available.");
                }
                else
                {
                    branch_id.Add(results.results[i].branch_id.ToString());
                }
                if(results.results[i].airport == null)
                {
                    airport.Add("No information available.");
                }
                else
                {
                    airport.Add(results.results[i].airport.ToString());
                }
                if (results.results[i].address.line1 == null || results.results[i].address.city == null || results.results[i].address.country == null)
                {
                    address.Add("No information available.");
                }
                else
                {
                    address.Add(results.results[i].address.line1.ToString() + ", " + results.results[i].address.city.ToString() + ", " + results.results[i].address.country.ToString());
                }
                
            }
        }

        
        private string AdjustDateToAPI(string date)
        { // from (dd.mm.yyyy) to (yyyy-mm-dd)
            string[] str = date.Split('.');
            string newDate = str[2] + "-" + str[1] + "-" + str[0];

            return newDate;
        }

        private string FixDate(string date)
        { // from (yyyy-mm-dd) to (dd.mm.yyyy)
            return date.Substring(8, 2) + "." + date.Substring(5, 2) + "." + date.Substring(0, 4);
        }

        private void BuildURL()
        {
            searchURL = "https://api.sandbox.amadeus.com/v1.2/cars/search-airport?apikey=" + apikey + "&location=" + LocationCodeCity + "&pick_up=" + PickupDate + "&drop_off=" + DropoffDate;
        }

        private void GetJson()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    this.json = wc.DownloadString(searchURL).ToString();
                }
                catch (WebException)
                {
                    response = "No results were found.";
                }
            }

        }

        public string GetLocation(string code, string stage) // this function for the google map in the result search
        {
            string url = "https://api.sandbox.amadeus.com/v1.2/location/" + code + "?apikey=" + apikey;

            string codeJson;
            using (WebClient wc = new WebClient())
            {
                codeJson = wc.DownloadString(url).ToString();
            }

            dynamic results = JsonConvert.DeserializeObject(codeJson);
            if (stage == "firstStage")
                return results.airports[0].city_code.ToString();

            else // stage == "secondStage"
                return results.city.location.latitude.ToString() + "," + results.city.location.longitude.ToString();
        }

        // Get Functions
        public string GetPickupDate() { return FixDate(PickupDate); }
        public string GetDropoffDate() { return FixDate(DropoffDate); }
        public string GetLocationCodeCity() { return LocationCodeCity; }

        public List<string> GetProvider() { return this.provider; }
        public List<string> GetAddress() { return this.address; }
        public List<string> GetAirport() { return this.airport; }
        public List<string> GetBranch() { return this.branch_id; }
        public List<double> GetLatitude() { return this.latitude; }
        public List<double> GetLongitude() { return this.longitude; }

        public List<CarRentVM> GetCars() { return this.cars; }

        public string GetResponse() { return this.response; }
    }
}