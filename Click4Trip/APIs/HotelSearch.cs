using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Click4Trip.APIs
{
    public class HotelSearch
    {
        private string apikey;
        private string location;
        private string check_in;
        private string check_out;
        //More search option are available if needed
        private string searchURL;

        public HotelSearch()
        {
            apikey = "FsvHjlWHVR56hqdQ46ibM2M6JRpbM8EZ"; // michael new api 28/05
        }
        public void FillData(string code, string sdate, string edate)
        {
            location = code;
            check_in = FixDate(sdate);
            check_out = FixDate(edate);
        }

        public string FixDate(string date)
        { 
            string[] str = date.Split('.');
            string newDate = str[2] + "-" + str[1] + "-" + str[0];

            return newDate; // (yyyy-mm-dd)
        }

        public void BuildURL()
        {
            searchURL = "https://api.sandbox.amadeus.com/v1.2/hotels/search-airport?apikey=" + apikey + "&location=" + location + "&check_in=" + check_in + "&check_out=" + check_out;
        }
        public string GetJson()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(searchURL);
                return json.ToString();
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

        public string GetSearchURL() { return this.searchURL; }
    }
}