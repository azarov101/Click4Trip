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
    public class FlightSearch
    {
        // general fields
        private string departureDate;
        private string returnDate;
        private string originCountry;
        private string originCity;
        private string destinationCountry;
        private string destinationCity;
        private string originCodeCity;
        private string destinationCodeCity;

        // fields for each flight
        private List<FlightRouteVM> outbound;
        private List<FlightRouteVM> inbound;
        private List<string> seatsRemaining;
        private List<string> airline;
        private List<double> price;

        // class helper fields
        private string apikey;
        private string searchURL;
        private string json;
        private string response; // to know if there was an error with the request

        public FlightSearch()
        {
            apikey = "FsvHjlWHVR56hqdQ46ibM2M6JRpbM8EZ"; // michael new api 28/05

            //departure_date = new List<string>();
            //return_date = new List<string>();
            outbound = new List<FlightRouteVM>();
            inbound = new List<FlightRouteVM>();
            seatsRemaining = new List<string>();
            airline = new List<string>();
            price = new List<double>();

            response = "";
        }

        public void FillData(string origin, string destination, string startDate, string endDate, string originCountry, string originCity, string destCountry, string destCity)
        {
            this.departureDate = AdjustDateToAPI(startDate);
            this.returnDate = AdjustDateToAPI(endDate);
            this.originCity = originCity;
            this.originCountry = originCountry;
            this.destinationCity = destCity;
            this.destinationCountry = destCountry;
            this.originCodeCity = FixCode(origin);
            this.destinationCodeCity = FixCode(destination);

            BuildURL();
            GetJson();

            if (response != "") // there was an error with the request
                return;

            dynamic results = JsonConvert.DeserializeObject(json);
            
            for (int i = 0; i < results.results.Count; ++i) 
            {
                FlightRouteVM temp_outbound = new FlightRouteVM();
                FlightRouteVM temp_inbound = new FlightRouteVM();

                temp_outbound.DepartureDate = FixDate(results.results[i].itineraries[0].outbound.flights[0].departs_at.ToString().Substring(0, 10));
                temp_outbound.DepartureTime = results.results[i].itineraries[0].outbound.flights[0].departs_at.ToString().Substring(11, 5);
                temp_outbound.ArrivalDate = FixDate(results.results[i].itineraries[0].outbound.flights[0].arrives_at.ToString().Substring(0, 10));
                temp_outbound.ArrivalTime = results.results[i].itineraries[0].outbound.flights[0].arrives_at.ToString().Substring(11, 5);
                temp_outbound.DepartureAirportCode = results.results[i].itineraries[0].outbound.flights[0].origin.airport;
                temp_outbound.ArrivalAirportCode = results.results[i].itineraries[0].outbound.flights[0].destination.airport;
                temp_outbound.FlightNumber = results.results[i].itineraries[0].outbound.flights[0].flight_number;
                temp_outbound.DepartureAirportName = GetAirportName(results.results[i].itineraries[0].outbound.flights[0].origin.airport.ToString());
                temp_outbound.ArrivalAirportName = GetAirportName(results.results[i].itineraries[0].outbound.flights[0].destination.airport.ToString());
                temp_outbound.DepartureDay = ToolsClass.DayOfTheWeek(FixDate(results.results[i].itineraries[0].outbound.flights[0].departs_at.ToString().Substring(0, 10)));
                temp_outbound.ArrivalDay = ToolsClass.DayOfTheWeek(FixDate(results.results[i].itineraries[0].outbound.flights[0].arrives_at.ToString().Substring(0, 10)));
                temp_outbound.TimeDuration = ToolsClass.TimeDifferences(results.results[i].itineraries[0].outbound.flights[0].departs_at.ToString().Substring(11, 5), results.results[i].itineraries[0].outbound.flights[0].arrives_at.ToString().Substring(11, 5));

                temp_inbound.DepartureDate = FixDate(results.results[i].itineraries[0].inbound.flights[0].departs_at.ToString().Substring(0, 10));
                temp_inbound.DepartureTime = results.results[i].itineraries[0].inbound.flights[0].departs_at.ToString().Substring(11, 5);
                temp_inbound.ArrivalDate = FixDate(results.results[i].itineraries[0].inbound.flights[0].arrives_at.ToString().Substring(0, 10));
                temp_inbound.ArrivalTime = results.results[i].itineraries[0].inbound.flights[0].arrives_at.ToString().Substring(11, 5);
                temp_inbound.DepartureAirportCode = results.results[i].itineraries[0].inbound.flights[0].origin.airport;
                temp_inbound.ArrivalAirportCode = results.results[i].itineraries[0].inbound.flights[0].destination.airport;
                temp_inbound.FlightNumber = results.results[i].itineraries[0].inbound.flights[0].flight_number;
                temp_inbound.DepartureAirportName = GetAirportName(results.results[i].itineraries[0].inbound.flights[0].origin.airport.ToString());
                temp_inbound.ArrivalAirportName = GetAirportName(results.results[i].itineraries[0].inbound.flights[0].destination.airport.ToString());
                temp_inbound.DepartureDay = ToolsClass.DayOfTheWeek(FixDate(results.results[i].itineraries[0].inbound.flights[0].departs_at.ToString().Substring(0, 10)));
                temp_inbound.ArrivalDay = ToolsClass.DayOfTheWeek(FixDate(results.results[i].itineraries[0].inbound.flights[0].arrives_at.ToString().Substring(0, 10)));
                temp_inbound.TimeDuration = ToolsClass.TimeDifferences(results.results[i].itineraries[0].inbound.flights[0].departs_at.ToString().Substring(11, 5), results.results[i].itineraries[0].inbound.flights[0].arrives_at.ToString().Substring(11, 5));

                outbound.Add(temp_outbound);
                inbound.Add(temp_inbound);

                seatsRemaining.Add(results.results[i].itineraries[0].inbound.flights[0].booking_info.seats_remaining.ToString());
                airline.Add(results.results[i].itineraries[0].inbound.flights[0].marketing_airline.ToString());
                price.Add(Convert.ToDouble(results.results[i].fare.total_price.ToString()));
            }
        }

        private string GetAirportName(string code)
        {
            string url = "https://api.sandbox.amadeus.com/v1.2/location/" + code + "?apikey=" + apikey;

            string codeJson;
            using (WebClient wc = new WebClient())
            {
                codeJson = wc.DownloadString(url).ToString();
            }

            dynamic results = JsonConvert.DeserializeObject(codeJson);
         
            object a = results.airports;
            if (a == null)
                return "No airport name";           

            try {            
                return results.airports.name.ToString();
            }
            catch (RuntimeBinderException) { return results.airports[0].name.ToString(); }
        }

        public string GetLocation() // this function for the google map in the result search
        {
            string url = "https://api.sandbox.amadeus.com/v1.2/location/" + destinationCodeCity + "?apikey=" + apikey;

            string codeJson;
            using (WebClient wc = new WebClient())
            {
                codeJson = wc.DownloadString(url).ToString();
            }

            dynamic results = JsonConvert.DeserializeObject(codeJson);

            return results.city.location.latitude.ToString() + "," + results.city.location.longitude.ToString();
        }

        private string FixCode(string code)
        {
            string url = "https://api.sandbox.amadeus.com/v1.2/location/" + code + "?apikey=" + apikey;

            string codeJson;
            using (WebClient wc = new WebClient())
            {
                codeJson = wc.DownloadString(url).ToString();
            }

            dynamic results = JsonConvert.DeserializeObject(codeJson);

            return results.airports[0].city_code.ToString();
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
            searchURL = "https://api.sandbox.amadeus.com/v1.2/flights/low-fare-search?apikey=" + apikey + "&origin=" + originCodeCity + "&destination=" + destinationCodeCity + "&departure_date=" + departureDate + "&return_date=" + returnDate + "&nonstop=true";            
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

        // Get Functions
        public string GetDepartureDate() { return FixDate(departureDate); }
        public string GetReturnDate() { return FixDate(returnDate); }

        public string GetOriginCountry() { return this.originCountry; }
        public string GetOriginCity() { return this.originCity; }
        public string GetDestinationCountry() { return this.destinationCountry; }
        public string GetDestinationCity() { return this.destinationCity; }
        public string GetOriginCodeCity() { return this.originCodeCity; }
        public string GetDestinationCodeCity() { return this.destinationCodeCity; }

        public List<FlightRouteVM> GetOutbound() { return this.outbound; }
        public List<FlightRouteVM> GetInbound() { return this.inbound; }
        public List<string> GetSeatsRemaining() { return this.seatsRemaining; }
        public List<string> GetAirline() { return this.airline; }
        public List<double> GetPrice() { return this.price; }

        public string GetResponse() { return this.response; }
    }
}