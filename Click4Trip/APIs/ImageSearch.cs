using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Click4Trip.GoogleAPI
{
    public class ImageSearch
    {
        private string url;
        private string apikey;
        private string hotelName;
        private string json;
        private string photo_reference;
        private string imageURL;
        private string imageMaxWidth;
        /* */
        private string place_id;
        private string detailsURL;
        private List<string> photo_references;
        private List<string> images;
        private string response;

        public ImageSearch()
        {
            imageMaxWidth = "400";
            photo_references = new List<string>();
            images = new List<string>();
            response = "";
        }

        public ImageSearch(string newMaxWidth) // this option for bigger images
        {
            imageMaxWidth = newMaxWidth;
            photo_references = new List<string>();
            images = new List<string>();
            response = "";
        }

        private void SetAPI()
        {
            //apikey = "AIzaSyBByfP1oxhh9kKaoTU0HQp82jQ5aLjYGbQ"; // Dani API KEY SECRET
            // apikey = "AIzaSyA6OjeMHlWAfl9X6GzMlWBuDCrkSFVgv7E"; // Michael API KEY SECRET
            //apikey = "AIzaSyA7zYjhLLgP1tnTfQQHiyDoH6LJdurBIzQ"; // Michael API KEY SECRET
            apikey = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
            url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + hotelName + "&key=" + apikey;
        }

        public void SetHotelName(string name)
        {
            this.hotelName = name.Replace(" ", "+");
            SetAPI();
            GetJson("url");
            if (response != "")
                return;

            dynamic results = JsonConvert.DeserializeObject(json);
            if (results.status != "ZERO_RESULTS")
            {
                photo_reference = results.results[0].photos[0].photo_reference;
                imageURL = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=" + imageMaxWidth + "&photoreference=" + photo_reference + "&key=" + apikey;
                place_id = results.results[0].place_id;
                detailsURL = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + place_id + "&key=" + apikey;
            }
            else
                imageURL = "https://i.imgur.com/5RGjbm5.jpg";
        }

        private void GetJson(string type)
        {
            string newType = "";
            if (type == "url") { newType = url; }
            else if (type == "detailsURL") { newType = detailsURL; }

            using (WebClient wc = new WebClient())
            {
                try
                {
                    this.json = wc.DownloadString(newType).ToString();
                }
                catch (ArgumentNullException)
                {
                    response = "No results were found.";
                }
            }
        }

        public string GetImage()
        {
            return imageURL;
        }
        public string GetKey()
        {
            return apikey;
        }

        public void SavePhotoReferences()
        {
            GetJson("detailsURL");
            dynamic results = JsonConvert.DeserializeObject(json);
            if (results.status != "ZERO_RESULTS")
            {
                for (int i=0; i < results.result.photos.Count; ++i)
                {
                    string tempPR = results.result.photos[i].photo_reference;
                    photo_references.Add(tempPR);
                    string tempURL = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&photoreference=" + photo_references[i] + "&key=" + apikey;
                    images.Add(tempURL);
                }             
            }
        }
        public List<string> GetImages()
        {
            return images;
        }
    }
}