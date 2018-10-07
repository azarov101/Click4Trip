using Click4Trip.APIs;
using Click4Trip.DAL;
using Click4Trip.GoogleAPI;
using Click4Trip.Models;
using Click4Trip.Tools;
using Click4Trip.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Click4Trip.Controllers
{
    public class HotelController : Controller
    {
        //                                               Index page for hotels (include search)
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Index()
        {
            HotelIndexVM hivm = new HotelIndexVM();
            DataLayer dl = new DataLayer();

            List<string>codes = (from u in dl.locations select u.code).ToList<string>();
            var rand = new Random();
            while(hivm.hotels.Count<6)
            {
                string code = codes.ElementAt(rand.Next(codes.Count()));
                HotelSearch hs = new HotelSearch();

                hs.FillData(code, "15.07.2018", "18.07.2018");
                hs.BuildURL();
                string json = hs.GetJson();

                dynamic results = JsonConvert.DeserializeObject(json); // convert incoming data to json form
                if (results.results.Count > 0)
                {
                    ImageSearch image = new ImageSearch();

                    try
                    {
                        image.SetHotelName(results.results[0].property_name.ToString()); // initialize object by GoogleAPI

                    }
                    catch(Exception)
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
                        hotelLink = results.results[0]._links.more_rooms_at_this_hotel.href,
                        hotelName = results.results[0].property_name,
                        hotelDescription = results.results[0].marketing_text,
                        rating = newRating,
                        hotelPrice = results.results[0].total_price.amount,
                        hotelImage = image.GetImage() // image is an object
                    };
                    hivm.hotels.Add(temp);
                }
            }
            hivm.countries = (from u in dl.locations
                              select u.country).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            hivm.cities = (from u in dl.locations
                           where u.country == hivm.countries.FirstOrDefault()
                           select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();
            return View(hivm);
        }

        public ActionResult GetCitiesByJSON(string Country)
        {
            DataLayer dl = new DataLayer();
            List<string> cities = (from u in dl.locations
                                   where u.country.ToLower() == Country.ToLower()
                                   select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitSearchGeneral(GeneralIndexVM obj)
        {
            HotelIndexVM hotel = new HotelIndexVM()
            {
                city = obj.desCity,
                country = obj.desCountry,
                sdate = obj.startDate,
                edate = obj.endDate,
            };
            return SubmitSearch(hotel);
        }

        [HttpPost]
        public ActionResult SubmitSearch(HotelIndexVM hotelsData)
        {
            DataLayer dl = new DataLayer();

            hotelsData.code = (from u in dl.locations
                               where u.city.ToLower()== hotelsData.city.ToLower()
                               select u.code).ToList<string>().FirstOrDefault();

            HotelSearch hs = new HotelSearch();

            hs.FillData(hotelsData.code, hotelsData.sdate, hotelsData.edate);
            hs.BuildURL();
            string json = hs.GetJson();

            dynamic results = JsonConvert.DeserializeObject(json); // convert incoming data to json form
            SearchResultsVM sr = new SearchResultsVM();
            sr.hotelSearchResultsVM = new List<HotelSearchResultsVM>();
            sr.startdDate = hotelsData.sdate;
            sr.endDate = hotelsData.edate;
            sr.originalLink = hs.GetSearchURL(); // to fix the api room problem
            sr.destination = hotelsData.city + ", " + hotelsData.country;

            if (results.results.Count != 0)
            { // for the google map
                string realCode = hs.GetLocation(hotelsData.code, "firstStage");
                string[] loc = hs.GetLocation(realCode, "secondStage").Split(',');
                sr.latitude = Double.Parse(loc[0]);
                sr.longitude = Double.Parse(loc[1]);
            }

            sr.nights = ToolsClass.getNumOfNights(ToolsClass.getDate(sr.startdDate), ToolsClass.getDate(sr.endDate));

            for (int i = 0; i < results.results.Count; ++i)
            {
                ImageSearch image = new ImageSearch();

                try
                {
                    image.SetHotelName(results.results[i].property_name.ToString()); // initialize object by GoogleAPI

                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    return SubmitSearch(hotelsData);
                }

                int newRating = -1; // initialize to -1
                try
                {
                    if (results.results[i].awards[0].rating != null)
                        newRating = results.results[i].awards[0].rating;
                }
                catch (Exception) { }
                if (newRating == -1)
                    newRating = GenerateRating();

                HotelSearchResultsVM temp = new HotelSearchResultsVM()
                {
                    hotelLink = results.results[i]._links.more_rooms_at_this_hotel.href,
                    hotelName = results.results[i].property_name,
                    hotelDescription = results.results[i].marketing_text,
                    rating = newRating,
                    hotelPrice = results.results[i].total_price.amount,
                    hotelImage = image.GetImage() // image is an object
                };
                sr.hotelSearchResultsVM.Add(temp);
            }
            return View("SearchResults", sr);
        }

        // TEMP ActionResult
        public ActionResult SearchResults()
        {
            HotelIndexVM boobs = new HotelIndexVM()
            {
                city = "London",
                country = "Uk",
                edate = "29.05.2018",
                sdate = "27.05.2018",
            };
            return SubmitSearch(boobs);
        }

        private int GenerateRating()
        {
            Random rnd = new Random();
            return rnd.Next(3, 6); // creates a number between 3 and 5

        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               View Hotel Info
        //-----------------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult ViewHotelInfoGeneral(GeneralIndexVM obj)
        {
            SearchResultsVM hotel = new SearchResultsVM()
            {
                destination = obj.desCountry,
                endDate = obj.endDate,
                startdDate = obj.startDate,
                nights = obj.nights,
                originalLink = obj.originalLink,
                selectedHotel = 0,
                hotelRating = obj.hotelRating,
                hotelId = obj.hotelId,
            };
            return ViewHotelInfo(hotel);
        }

        [HttpPost]
        public ActionResult ViewHotelInfo(SearchResultsVM srvm)
        {
            var json = "";
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(srvm.hotelId).ToString();
            }
            dynamic results = JsonConvert.DeserializeObject(json);

            // save some data from json to list. later we will save this data in the object we will pass to the view
            List<string> contactLST = HotelDetailsHelper1(results, "contacts");
            List<string> amenitiesLST = HotelDetailsHelper1(results, "amenities");
            List<RoomVM> roomLST = HotelDetailsHelper2(results);

            if (roomLST.Count == 0) // we need to fix the api
                roomLST = HotelDetailsHelper3(srvm.originalLink, srvm.selectedHotel);

            // load reviews
            DataLayer dl = new DataLayer();

            string hotelName = results.property_name;

            List<HotelReview> hotelReviewsList = (from u in dl.hotelReviews
                                                  where u.HotelName.ToLower() == hotelName.ToLower()
                                                  select u).ToList<HotelReview>();

            // search for customer name (by email)
            List<string> customersName = new List<string>();
            for (int i = 0; i < hotelReviewsList.Count; ++i)
            {
                string tempEmail = hotelReviewsList[i].CustomerEmail;
                string customerName = (from u in dl.customers
                                       where u.Email.ToLower() == tempEmail.ToLower()
                                       select u.FullName).ToList<string>()[0];
                customersName.Add(customerName);
            }


            // create HotelOrderDetailsVM to pass to the view
            ImageSearch image = new ImageSearch();
            image.SetHotelName(results.property_name.ToString()); // initialize object by GoogleAPI
            image.SavePhotoReferences();
            List<string> imageLST = image.GetImages();

            HotelOrderDetailsVM hotelOrderDetailsVM = new HotelOrderDetailsVM()
            {
                destination = srvm.destination,
                hotelName = results.property_name,
                address = "ADDRESS: " + results.address.line1 + ", " + results.address.city,
                contact = contactLST,
                amenities = amenitiesLST,
                rating = srvm.hotelRating,
                latitude = results.location.latitude,
                longitude = results.location.longitude,
                room = roomLST,
                image = imageLST,
                apiKey = image.GetKey(),
                searchResultsVM = srvm,
                reviews = hotelReviewsList,
                customersName = customersName
            };

            return View("ViewHotelInfo", hotelOrderDetailsVM);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult ViewHotelInfo()
        {
            SearchResultsVM srvm = new SearchResultsVM()
            {
                hotelId = "https://api.sandbox.amadeus.com/v1.2/hotels/UZBOS174?apikey=qEM5DE19KoNf7p4xwiuLealWmaUzTTG5&check_in=2018-08-15&check_out=2018-08-16&referrer=more_rooms_at_this_hotel",
                startdDate = "15.08.2018",
                endDate = "16.08.2018",
                nights = 1,
                hotelRating = "4",
            };
            return ViewHotelInfo(srvm);
        }

        private List<string> HotelDetailsHelper1(dynamic results, string type)
        { // this function help to get "contacts" and "amenities" from the JSON

            List<string> lst = new List<string>();

            if (type == "amenities")
            {
                int count = 10;
                if (results.amenities.Count <= 10)
                    count = results.amenities.Count;

                for (int i = 0; i < count; ++i)
                {
                    string temp = results.amenities[i].description;
                    lst.Add(temp);
                }
            }

            else if (type == "contacts")
            {
                for (int i = 0; i < results.contacts.Count; ++i)
                {
                    string temp = results.contacts[i].type + ": " + results.contacts[i].detail;
                    if (temp.Length > 9 && temp.Substring(0, 8) == "FAX: FAX")
                        temp = temp.Substring(5);
                    lst.Add(temp);
                }
            }
            return lst;
        }

        private List<RoomVM> HotelDetailsHelper2(dynamic results)
        { // this function help to get the room description from the JSON

            List<RoomVM> lst = new List<RoomVM>();

            for (int i = 0; i < results.rooms.Count; ++i)
            {
                List<string> roomTypeTEMP = new List<string>();
                string str = "Room type: " + results.rooms[i].room_type_info.room_type;
                roomTypeTEMP.Add(str);
                str = "Bed type: " + results.rooms[i].room_type_info.bed_type;
                roomTypeTEMP.Add(str);
                str = "Number of beds: " + results.rooms[i].room_type_info.number_of_beds;
                roomTypeTEMP.Add(str);

                RoomVM temp = new RoomVM()
                {
                    roomPrice = results.rooms[i].total_amount.amount,
                    roomDescription = results.rooms[i].descriptions[1],
                    roomType = roomTypeTEMP
                };
                    
                lst.Add(temp);
            }
            
            return lst;
        }

        private List<RoomVM> HotelDetailsHelper3(string link, int index)
        { // this function help to get the room description from the JSON

            var json = "";
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(link);
            }

            dynamic results = JsonConvert.DeserializeObject(json);

            List<RoomVM> lst = new List<RoomVM>();

            for (int i = 0; i < results.results[index].rooms.Count; ++i)
            {
                List<string> roomTypeTEMP = new List<string>();
                string str = "Room type: " + results.results[index].rooms[i].room_type_info.room_type;
                roomTypeTEMP.Add(str);
                str = "Bed type: " + results.results[index].rooms[i].room_type_info.bed_type;
                roomTypeTEMP.Add(str);
                str = "Number of beds: " + results.results[index].rooms[i].room_type_info.number_of_beds;
                roomTypeTEMP.Add(str);

                RoomVM temp = new RoomVM()
                {
                    roomPrice = results.results[index].rooms[i].total_amount.amount,
                    roomDescription = results.results[index].rooms[i].descriptions[1],
                    roomType = roomTypeTEMP
                };

                lst.Add(temp);
            }

            return lst;
        }

        [HttpPost]
        public ActionResult SubmitRoomComposition(HotelOrderDetailsVM rc)
        {
            List<RoomVM> rooms = new List<RoomVM>();
            RoomVM room1 = new RoomVM()
            {
                roomPrice = rc.roomPrice1,
                roomDescription = rc.roomType1,
                roomComposition = rc.roomComposition1
            };
            RoomVM room2 = new RoomVM()
            {
                roomPrice = rc.roomPrice2,
                roomDescription = rc.roomType2,
                roomComposition = rc.roomComposition2
            };
            RoomVM room3 = new RoomVM()
            {
                roomPrice = rc.roomPrice3,
                roomDescription = rc.roomType3,
                roomComposition = rc.roomComposition3
            };
            rooms.Add(room1);
            rooms.Add(room2);
            rooms.Add(room3); // always pass 3 rooms

            HotelCheckoutVM hotelCheckoutVM = new HotelCheckoutVM()
            {
                hotelName = rc.hotelName,
                numberOfRooms = rc.numberOfRooms,
                room = rooms,
                startDate = rc.startDate,
                endDate = rc.endDate,
                hotelAddress = rc.address
            };

            return View("HotelCheckout", hotelCheckoutVM);
        }

        [HttpPost]
        public ActionResult SubmitOrder(HotelCheckoutVM hc)
        {           
            DataLayer dl = new DataLayer();

            DateTime today = DateTime.Today;
            string date = today.ToString("dd/MM/yyyy").Replace("/",".");
            double totalFee = Convert.ToDouble(hc.totalFee.ToString().Substring(1, hc.totalFee.Length -1));
            bool sameCustomer = false;
            List<HotelOrderDetails> hotelOrderDetailsList = new List<HotelOrderDetails>();

            // first check if customer already exists in table
            List<Customer> customerFromDB = (from u in dl.customers
                                             where u.Email.ToLower() == hc.email.ToLower()
                                             select u).ToList<Customer>();

            if (customerFromDB.Count != 0) // if customer already exists
            {
                // check if the input is the same as the customer data
                if (customerFromDB[0].FullName != hc.customerName || customerFromDB[0].ID != hc.id)
                {
                    TempData["error"] = "This email address already exist in our records with different name and/or different id!";
                    return View("HotelCheckout", hc);
                }
                else if (customerFromDB[0].Location != hc.address || customerFromDB[0].PhoneNumber != hc.phone)
                {
                    // update: delete this row and later add new customer row
                    dl.customers.Remove(customerFromDB[0]);
                    dl.SaveChanges();
                }
                // else: same row - no need to update
                else
                    sameCustomer = true;
            }

            string[] destination = hc.destination.Split(',');
            string destCountry = destination[1];
            if (destination[1][0] == ' ')
                destCountry = destination[1].Substring(1, destination[1].Length - 1);

            Order order = new Order()
            {
                CreditCard = hc.creditCard,
                CustomerEmail = hc.email,
                OrderDate = date,
                Status = 0,
                TotalPayment = totalFee,
                Type = "Hotel",
                OrderDestination = destCountry,
            };

            dl.orders.Add(order); // adding in memory and not to DB //
            dl.SaveChanges();
            int INVOICE = order.InvoiceNumber;

            HotelOrder hotelOrder = new HotelOrder()
            {
                InvoiceID = INVOICE,
                StartDate = hc.startDate,
                EndDate = hc.endDate,
                HotelName = hc.hotelName,
                NumOfRooms = hc.numberOfRooms,
                Reviewed = 0,
            };

            dl.hotelOrders.Add(hotelOrder); // adding in memory and not to DB //

            for (int i=0; i<hc.numberOfRooms; ++i)
            {
                HotelOrderDetails hotelOrderDetails = new HotelOrderDetails()
                {
                    RoomType = hc.room[i].roomDescription,
                    PaymentForRoom = hc.room[i].roomPrice,
                    RoomComposition = hc.room[i].roomComposition,
                    Invoice = INVOICE                   
                };
                //hotelOrderDetailsList.Add(hotelOrderDetails);
                dl.hotelOrderDetails.Add(hotelOrderDetails); // adding in memory and not to DB //
            }

            if (!sameCustomer)
            {
                Customer customer = new Customer()
                {
                    Email = hc.email,
                    FullName = hc.customerName,
                    Location = hc.location,
                    ID = hc.id,
                    PhoneNumber = hc.phone
                };

                dl.customers.Add(customer); // adding in memory and not to DB //
            }

            dl.SaveChanges();

            // create object to sent to recipt view
           
            return RedirectToAction("ViewOrderReceipt", "Support", new { invoice = INVOICE });
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult HotelCheckout()
        {
            List<RoomVM> rooms = new List<RoomVM>();
            RoomVM room1 = new RoomVM()
            {
                roomPrice = 250.22,
                roomDescription = "bla bla bla blabla bla bla bla",
                roomComposition = "Single"
            };
            RoomVM room2 = new RoomVM()
            {
                roomPrice = 581.85,
                roomDescription = "bla bla bla bla",
                roomComposition = "Adult + 2 Children"
            };
            rooms.Add(room1);
            rooms.Add(room2);
            rooms.Add(new RoomVM()); // always pass 3 rooms

            HotelCheckoutVM hotelCheckoutVM = new HotelCheckoutVM()
            {
                hotelName = "Globales Acis Y Galatea",
                numberOfRooms = 2,
                room = rooms,
                startDate = "15.06.2018",
                endDate = "18.06.2018",
                hotelAddress = "Galatea 6 Madrid"
            };

            return View(hotelCheckoutVM);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult ViewOrder()
        {
            List<RoomVM> roomsList = new List<RoomVM>();
            RoomVM tempRoom1 = new RoomVM()
            {
                roomDescription = "bla bla bla",
                roomComposition = "Single",
                roomPrice = 256.12,
            };
            RoomVM tempRoom2 = new RoomVM()
            {
                roomDescription = "what are you saying to me?",
                roomComposition = "Adult + Child",
                roomPrice = 812.10,
            };
            roomsList.Add(tempRoom1);
            roomsList.Add(tempRoom2);

            ViewHotelOrderVM viewHotelOrder = new ViewHotelOrderVM()
            {
                creditCardNumber = "1111222233334444",
                customerId = "123456789",
                customerName = "Daniel Mashukov",
                email = "dani@gmail.com",
                startDate = "15.06.2018",
                endDate = "18.06.2018",
                //hotelAddress = "Galatea 6 Madrid",
                hotelName = "Globales Acis Y Galatea",
                invoice = 12,
                orderDate = "08.04.2018",
                status = 0,
                totalPrice = 1560.2,
                rooms = roomsList
            };
            return RedirectToAction("ViewOrderReceipt", "Support", viewHotelOrder);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Write Review
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult WriteReview() => View();

        [HttpPost]
        public ActionResult SubmitIdentify(IdentifyVM ivm)
        {
            DataLayer dl = new DataLayer();
            List<Order> orderlist = (from x in dl.orders
                                        where x.CreditCard == ivm.credit && x.InvoiceNumber == ivm.InvoiceId && (x.Type=="Hotel" || x.Type=="Package")
                                        select x).ToList<Order>();

            if (orderlist.Count == 0)
            {
                ViewData["msg"] = "Invalid Credit Card and/or Invoice!";
                return View("WriteReview", ivm);
            }

            if (orderlist.First().Status!=1)
            {
                ViewData["msg"] = "Order is not confirmed";
                return View("WriteReview", ivm);
            }

            if (orderlist.First().Type == "Hotel")
            {
                List<HotelOrder> horderlist = (from x in dl.hotelOrders
                                               where x.InvoiceID == ivm.InvoiceId
                                               select x).ToList<HotelOrder>();

                int inid = horderlist.First().InvoiceID;

                if (horderlist.First().Reviewed == 1)
                {
                    ViewData["msg"] = "You Already Reviewed This Order!";
                    return View("WriteReview", ivm);
                }

                if (!ToolsClass.iskDateValidForReview(ToolsClass.getDate(horderlist.First().EndDate)))
                {
                    ViewData["msg"] = "You Cannot Review This Order Yet! (wait untill the end)";
                    return View("WriteReview", ivm);
                }

                TempData["hotelName"] = horderlist.First().HotelName;
                TempData["ID"] = inid;
                TempData["email"] = orderlist.First().CustomerEmail;
                HotelReview hr = new HotelReview();
                hr.HotelName = horderlist.First().HotelName;
                hr.CustomerEmail = orderlist.First().CustomerEmail;

                hr.CustomerName = (from x in dl.customers
                                   where x.Email == hr.CustomerEmail
                                   select x.FullName).ToList<string>().FirstOrDefault();
                TempData["Cname"] = hr.CustomerName;
                return View("HotelReview", hr);
            }

            else // if package
            {
                List<PackageOrder> packagelist = (from x in dl.packageOrders
                                                   where x.InvoiceID == ivm.InvoiceId
                                                   select x).ToList<PackageOrder>();

                int inid = packagelist.First().InvoiceID;

                if (packagelist.First().Reviewed == 1)
                {
                    ViewData["msg"] = "You Already Reviewed This Order!";
                    return View("WriteReview", ivm);
                }

                if (!ToolsClass.iskDateValidForReview(ToolsClass.getDate(packagelist.First().ReturnDate)))
                {
                    ViewData["msg"] = "You Cannot Review This Order Yet! (wait untill the end)";
                    return View("WriteReview", ivm);
                }

                TempData["hotelName"] = packagelist.First().HotelName;
                TempData["ID"] = inid;
                TempData["email"] = orderlist.First().CustomerEmail;
                HotelReview hr = new HotelReview();
                hr.HotelName = packagelist.First().HotelName;
                hr.CustomerEmail = orderlist.First().CustomerEmail;

                hr.CustomerName = (from x in dl.customers
                                   where x.Email == hr.CustomerEmail
                                   select x.FullName).ToList<string>().FirstOrDefault();
                TempData["Cname"] = hr.CustomerName;
                return View("HotelReview", hr);
            }
        }

        [HttpPost]
        public ActionResult SubmitReview(HotelReview hv)
        {
            if (ModelState.IsValid)
            {
                DataLayer dl = new DataLayer();
                int id = Convert.ToInt32(TempData["ID"].ToString());

                List<HotelOrder> horderlist = (from x in dl.hotelOrders
                                                  where x.InvoiceID == id
                                               select x).ToList<HotelOrder>();

                List<PackageOrder> packageOrder = (from x in dl.packageOrders
                                                   where x.InvoiceID == id
                                               select x).ToList<PackageOrder>();

                foreach (HotelOrder ord in horderlist)
                    ord.Reviewed = 1;

                foreach (PackageOrder ord in packageOrder)
                    ord.Reviewed = 1;

                hv.HotelName = TempData["hotelName"].ToString();
                hv.CustomerEmail = TempData["email"].ToString();
                hv.CustomerName = TempData["Cname"].ToString();
                hv.ReviewDate = DateTime.Today.ToShortDateString().Replace('/','.');

                // research part:

                string rev = Tools.ToolsClass.fixReview(hv.Review);

                string url = "http://147.235.176.28/"+ rev;

                string codeJson;
                using (WebClient wc = new WebClient())
                {
                    codeJson = wc.DownloadString(url).ToString();
                }

                dynamic results = JsonConvert.DeserializeObject(codeJson);

                string pos= results.Positive.Value.ToString();
                string neg = results.Negative.Value.ToString();
                string sp = results.Spam.Value.ToString();

                pos = pos.Replace("%","").Replace("'","");
                neg = neg.Replace("%", "").Replace("'", "");
                sp = sp.Replace("%", "").Replace("'", "");

                double positive = Convert.ToDouble(pos);
                double negative = Convert.ToDouble(neg);
                double spam = Convert.ToDouble(sp);

                if (spam>=100)
                {
                    ViewData["msg"] = "Review detected as spam! Please avoid using bad words";
                    return View("WriteReview");
                }

                if (positive > negative)
                    hv.ReviewType = 0;

                else
                    hv.ReviewType = 1;

                dl.hotelReviews.Add(hv);
                dl.SaveChanges();
                ViewData["msgsc"] = "Review sent successfully!";
                return View("WriteReview");
            }
            return View("HotelReview", hv);
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}