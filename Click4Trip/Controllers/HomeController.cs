using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Click4Trip.ViewModel;
using Click4Trip.GoogleAPI;
using Click4Trip.APIs;
using System.Threading;
using Newtonsoft.Json;
using Click4Trip.Tools;

namespace Click4Trip.Controllers
{
    public class HomeController : Controller
    {
        //                                               Home Page
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Index() => View();

        public ActionResult NewIndex() {
            var url = Request.RawUrl;
            if (url == @"/")
            {
                Response.Redirect("/Home/NewIndex");
            }
            return View(new GeneralIndexVM());
        }

        [HttpGet]
        public ActionResult GetDestinationsByJson()
        {
            DataLayer dl = new DataLayer();

            // access DB ONCE(!) and get all the locations
            List<Locations> locations = (from u in dl.locations
                                         select u).ToList<Locations>();
           
            // create list grouped by countries
            List<string> countries = (from u in locations
                                         select u.country).ToList<string>().GroupBy(p => p)
                                         .Select(g => g.First()).OrderBy(q => q)
                                         .ToList();

            // create city list that connected to the countries
            List<string> cities = new List<string>();
            for (int i = 0; i < locations.Count; ++i)
            {
                string temp = locations[i].country + "," + locations[i].city + "," + locations[i].code;
                cities.Add(temp);
            }

            // create object that will contain both lists
            List<List<string>> obj = new List<List<string>>
            {
                countries, cities
            };


            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        private List<Locations> GetLocations(List<string> str)
        { // this function convert the list of strings to list of locations
            List<Locations> locations = new List<Locations>();
            for (int i = 0; i < str.Count; ++i)
            {
                string[] location = str[i].Split(','); // country, city, code
                Locations temp = new Locations()
                {
                    country = location[0],
                    city = location[1],
                    code = location[2]
                };
                locations.Add(temp);
            }
            return locations;
        }

        public ActionResult GetRandomHotelsByJson(List<string> loc)
        {
            HotelIndexVM hivm = new HotelIndexVM();
            DataLayer dl = new DataLayer();

            List<Locations> locations = GetLocations(loc); 
            var rand = new Random();

            string startDate = "13.08.2018";
            string endDate = "20.08.2018";
            hivm.nights = ToolsClass.getNumOfNights(ToolsClass.getDate(startDate), ToolsClass.getDate(endDate));

            while (hivm.hotels.Count < 4)
            {
                int random = rand.Next(0, locations.Count);
                string code = locations[random].code;
                HotelSearch hs = new HotelSearch();


                hs.FillData(code, startDate, endDate);
                hs.BuildURL();
                string json = hs.GetJson();
                string originalLink = hs.GetSearchURL();

                dynamic results = JsonConvert.DeserializeObject(json); // convert incoming data to json form
                if (results.results.Count > 0)
                {
                    ImageSearch image = new ImageSearch();

                    try
                    {
                        image.SetHotelName(results.results[0].property_name.ToString()); // initialize object by GoogleAPI

                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                        return Index();
                    }

                    int newRating = -1; // initialize to -1
                    try
                    {
                        newRating = results.results[0].awards[0].rating;
                    }
                    catch (Exception) { newRating = GenerateRating(); }
                    if (newRating == -1)
                        newRating = GenerateRating();

                    HotelSearchResultsVM temp = new HotelSearchResultsVM()
                    {
                        originalLink = originalLink,
                        startDate = startDate,
                        endDate = endDate,
                        location = locations[random].city + ", " + locations[random].country,
                        hotelLink = results.results[0]._links.more_rooms_at_this_hotel.href,
                        hotelName = results.results[0].property_name,
                        hotelDescription = results.results[0].marketing_text,
                        rating = newRating,
                        hotelPrice = results.results[0].total_price.amount,
                        hotelImage = image.GetImage() // image is an object
                    };
                    hivm.hotels.Add(temp);
                    locations.RemoveAt(random);
                }
            }
            return Json(hivm, JsonRequestBehavior.AllowGet);
        }

        private int GenerateRating()
        {
            Random rnd = new Random();
            return rnd.Next(3, 6); // creates a number between 3 and 5

        }

        private List<string> DataMining(string locApi)
        {
            DataLayer dl = new DataLayer();

            List<string> emails = (from u in dl.customers
                                   where u.Location == locApi
                                   select u.Email).ToList<string>();

            List<string> locations = new List<string>();
            foreach (string e in emails)
            {
                List<string> des = (from u in dl.orders
                                       where u.CustomerEmail == e
                                       select u.OrderDestination).ToList<string>();

                foreach (string s in des)
                    locations.Add(s);
            }

            HashSet<string> locSet = new HashSet<string>(locations);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string s in locSet)
                dict[s] = 0;

            foreach(string s in locations)
                dict[s] = dict[s]+1;

            List<string> best4Loc = new List<string>();

            if (dict.Count > 4)
            {
                List<int> arr = new List<int>(dict.Values);
                arr.Sort();
                foreach (string k in dict.Keys)
                    if (dict[k] == arr[arr.Count-1])
                    {
                        best4Loc.Add(k);
                        dict.Remove(k);
                        break;
                    }
                foreach (string k in dict.Keys)
                    if (dict[k] == arr[arr.Count - 2])
                    {
                        best4Loc.Add(k);
                        dict.Remove(k);
                        break;
                    }
                foreach (string k in dict.Keys)
                    if (dict[k] == arr[arr.Count - 3])
                    {
                        best4Loc.Add(k);
                        dict.Remove(k);
                        break;
                    }
                foreach (string k in dict.Keys)
                    if (dict[k] == arr[arr.Count - 4])
                    {
                        best4Loc.Add(k);
                        dict.Remove(k);
                        break;
                    }
            }

            else
            {
                best4Loc = new List<string>(dict.Keys);
                List<string> smartAgent_LST = (from u in dl.smartAgent
                                               select u.Location).ToList<string>();
                foreach (string s in best4Loc)
                    smartAgent_LST.Remove(s);

                Random rnd = new Random();

                for (int i = best4Loc.Count, j = smartAgent_LST.Count; i < 4; ++i, --j)
                {
                    int random = rnd.Next(0, j);
                    best4Loc.Add(smartAgent_LST[random]);
                    smartAgent_LST.RemoveAt(random);
                }
            }

            return best4Loc;
        }

        public ActionResult GetBestLocationsByJson(string locApi)
        {
            DataLayer dl = new DataLayer();

            // Data mining part:
            List<string> locations = DataMining(locApi);

            // get all rows from database
            List<SmartAgent> smartAgent_LST = (from u in dl.smartAgent
                                               select u).ToList<SmartAgent>();

            List<SmartAgentResultsVM> results = new List<SmartAgentResultsVM>(); // the list we will return to the view
            Random rnd = new Random();

            for (int k = 0; k < smartAgent_LST.Count; ++k) // this loop search for the location acordint to the data mining
            {
                string[] tempLoc = smartAgent_LST[k].Location.Split(',');
                string countryName = tempLoc[1];
                if (tempLoc.Length == 2 && tempLoc[0] == "Andorra")
                    countryName = tempLoc[0];
                else if (countryName[0] == ' ')
                    countryName = countryName.Substring(1);

                if (countryName.ToLower() == locations[0].ToLower() || countryName.ToLower() == locations[1].ToLower() ||
                    countryName.ToLower() == locations[2].ToLower() || countryName.ToLower() == locations[3].ToLower())
                {
                    SmartAgentResultsVM temp2 = new SmartAgentResultsVM()
                    {
                        Location = smartAgent_LST[k].Location,
                        Image = smartAgent_LST[k].Image,
                        Description = smartAgent_LST[k].Description,
                    };
                    results.Add(temp2);
                    smartAgent_LST.RemoveAt(k);
                }
            }

            if (results.Count < 4) // if not all the location were filled (could happen if the locations we have from the data mining are not all in the SmartAgent table)
            { // so add some random locations
                int size = 4 - results.Count;

                for (int i = 0, j = smartAgent_LST.Count; i < size; ++i, --j)
                {
                    int random = rnd.Next(0, j); // get random location

                    SmartAgentResultsVM temp = new SmartAgentResultsVM()
                    {
                        Location = smartAgent_LST[random].Location,
                        Image = smartAgent_LST[random].Image,
                        Description = smartAgent_LST[random].Description,
                    };
                    results.Add(temp);
                    smartAgent_LST.RemoveAt(random);
                }
            }

            else
            { // all the locations were filled by the data mining locations
                while (results.Count > 4) // if there were more then 4 locations that was added to the list, remove them
                {
                    results.RemoveAt(results.Count-1);
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        //                                               Login
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return View("NewIndex");
            return View(new UserPass());
        }

        [HttpPost]
        public ActionResult SubmitLogin(UserPass user)
        {
            if (ModelState.IsValid)
            {
                DataLayer dl = new DataLayer();
                List<UserPass> userPass = (from u in dl.userPass
                                             where u.Email.ToUpper() == user.Email.ToUpper()
                                             select u).ToList<UserPass>();

                if (userPass.Count == 1)
                {
                    List<UserSalt> userSalt= (from u in dl.userSalt
                                              where u.Email.ToUpper() == user.Email.ToUpper()
                                              select u).ToList<UserSalt>();
                    if (userSalt.Count == 1)
                    {
                        Encryption encryptionCheck = new Encryption();
                        if (encryptionCheck.ValidatePassword(user.Password, userPass.First().Password, userSalt.First().Salt))
                        {
                            FormsAuthentication.SetAuthCookie(userPass.First().Email, true);
                            return RedirectToAction("NewIndex", "Home");
                        }
                    }
                }
            }
            ViewData["msg"] = "Username and/or Password is incorrect!";
            return View("Login", user);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Logout
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("NewIndex", "Home");
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}