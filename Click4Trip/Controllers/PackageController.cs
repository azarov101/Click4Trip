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

namespace Click4Trip.Controllers
{
    public class PackageController : Controller
    {
        //                                               Index page for hotels (include search)
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Index()
        {
            PackageIndexVM pivm = new PackageIndexVM();
            DataLayer dl = new DataLayer();

            pivm.countries = (from u in dl.locations
                              select u.country).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            pivm.cities = (from u in dl.locations
                           where u.country == pivm.countries.FirstOrDefault()
                           select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();
            return View(pivm);
        }

        [HttpPost]
        public ActionResult SubmitSearchGeneral(GeneralIndexVM obj)
        {
            PackageIndexVM package = new PackageIndexVM()
            {
                composition = obj.composition,
                desCity = obj.desCity,
                desCountry = obj.desCountry,
                sdate = obj.startDate,
                edate = obj.endDate,
            };

            return SubmitSearch(package);
        }

        [HttpPost]
        public ActionResult SubmitSearch(PackageIndexVM packageData)
        {
            DataLayer dl = new DataLayer();

            packageData.desCode = (from u in dl.locations
                               where  u.city.ToLower() == packageData.desCity.ToLower()
                               select u.code).ToList<string>().FirstOrDefault();

            // HOTELS:
            HotelSearch hs = new HotelSearch();

            hs.FillData(packageData.desCode, packageData.sdate, packageData.edate);
            hs.BuildURL();
            string json = hs.GetJson();

            dynamic results = JsonConvert.DeserializeObject(json); // convert incoming data to json form
            SearchResultsVM sr = new SearchResultsVM();
            sr.hotelSearchResultsVM = new List<HotelSearchResultsVM>();
            sr.startdDate = packageData.sdate;
            sr.endDate = packageData.edate;
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
                    return SubmitSearch(packageData);
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
                    originalLink = hs.GetSearchURL(), // to fix the api room problem
                    hotelLink = results.results[i]._links.more_rooms_at_this_hotel.href,
                    hotelName = results.results[i].property_name,
                    hotelDescription = results.results[i].marketing_text,
                    rating = newRating,
                    hotelPrice = results.results[i].total_price.amount,
                    hotelImage = image.GetImage() // image is an object
                };
                sr.hotelSearchResultsVM.Add(temp); 
            }

            // FLIGHTS:
            FlightSearch flightSearch = new FlightSearch();
            flightSearch.FillData("TLV", packageData.desCode, packageData.sdate, packageData.edate, "Israel", "Tel Aviv", packageData.desCountry, packageData.desCity);

            // if no results were found (400: bad request)
            if (flightSearch.GetResponse() != "")
            {
                List<PackageDetails> packages = new List<PackageDetails>();
                packages.Add(new PackageDetails(){
                    flight = new FlightInfoVM()
                    {
                        departureDate = packageData.sdate,
                        returnDate = packageData.edate,
                        destinationCity = packageData.desCity,
                        destinationCountry = packageData.desCountry,
                        originCity = "Tel Aviv",
                        originCountry = "Israel",                        
                    }
                });

                PackageDetailsVM newObj = new PackageDetailsVM()
                {
                    composition = packageData.composition,
                    packages = packages,
                };
                return View("SearchResults", newObj);
            }

            // for the google map
            string[] loc = flightSearch.GetLocation().Split(',');
            double latitude = Double.Parse(loc[0]);
            double longitude = Double.Parse(loc[1]);

            // create object to pass to the view
            FlightSearchResultsVM flightSearchResultsVM = new FlightSearchResultsVM()
            {
                departureDate = flightSearch.GetDepartureDate(),
                destinationCity = flightSearch.GetDestinationCity(),
                destinationCodeCity = flightSearch.GetDestinationCodeCity(),
                destinationCountry = flightSearch.GetDestinationCountry(),
                inbound = flightSearch.GetInbound(),
                originCity = flightSearch.GetOriginCity(),
                originCodeCity = flightSearch.GetOriginCodeCity(),
                originCountry = flightSearch.GetOriginCountry(),
                outbound = flightSearch.GetOutbound(),
                price = flightSearch.GetPrice(),
                returnDate = flightSearch.GetReturnDate(),
                seatsRemaining = flightSearch.GetSeatsRemaining(),
                airline = flightSearch.GetAirline(),
            };

            PackageDetailsVM pdvm = new PackageDetailsVM();
            pdvm.composition = packageData.composition;
            var rand = new Random();
            foreach (HotelSearchResultsVM h in sr.hotelSearchResultsVM)
            {
                PackageDetails pd = new PackageDetails();
                pd.hotel = h;

                int n=rand.Next(flightSearchResultsVM.airline.Count());
                FlightInfoVM f = new FlightInfoVM()
                {
                    airline = flightSearchResultsVM.airline[n],
                    inbound= flightSearchResultsVM.inbound[n],
                    outbound = flightSearchResultsVM.outbound[n],
                    departureDate= flightSearchResultsVM.departureDate,
                    returnDate= flightSearchResultsVM.returnDate,
                    originCountry= flightSearchResultsVM.originCountry,
                    originCity= flightSearchResultsVM.originCity,
                    destinationCountry = flightSearchResultsVM.destinationCountry,
                    destinationCity = flightSearchResultsVM.destinationCity,
                    originCodeCity= flightSearchResultsVM.originCodeCity,
                    destinationCodeCity= flightSearchResultsVM.destinationCodeCity,
                    seatsRemaining= flightSearchResultsVM.seatsRemaining[n],
                    price= flightSearchResultsVM.price[n].ToString()
                };
                pd.flight = f;

                pd.price = (pd.hotel.hotelPrice + Convert.ToDouble(pd.flight.price)) * Tools.ToolsClass.getPricePercent(pdvm.composition);

                pdvm.packages.Add(pd);
            }

            pdvm.nights = sr.nights;

            pdvm.latitude = latitude;
            pdvm.longitude = longitude;

            return View("SearchResults", pdvm);
        }

        // TEMP ActionResult for static run
        public ActionResult SearchResults()
        {
            PackageIndexVM obj = new PackageIndexVM()
            {
                composition = "Couple",
                desCity = "London",
                desCountry = "UK",
                edate = "16.07.2018",
                sdate = "09.07.2018",
            };

            return SubmitSearch(obj);
        }

        private int GenerateRating()
        {
            Random rnd = new Random();
            return rnd.Next(3, 6); // creates a number between 3 and 5

        }

        private PackageIndexVM LoadCities()
        {
            PackageIndexVM fivm = new PackageIndexVM();
            DataLayer dl = new DataLayer();


            fivm.countries = (from u in dl.locations
                                    select u.country).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            fivm.cities = (from u in dl.locations
                                 where u.country == fivm.countries.FirstOrDefault()
                                 select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            return fivm;
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               View Package Info
        //-----------------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult ViewPackageInfo(PackageDetailsVM obj)
        {
            var json = "";
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(obj.packages[obj.selectedPack].hotel.hotelLink).ToString();
            }
            dynamic results = JsonConvert.DeserializeObject(json);

            // save some data from json to list. later we will save this data in the object we will pass to the view
            List<string> contactLST = HotelDetailsHelper1(results, "contacts");
            List<string> amenitiesLST = HotelDetailsHelper1(results, "amenities");
            List<RoomVM> roomLST = HotelDetailsHelper2(results);

            if (roomLST.Count == 0) // we need to fix the api
                roomLST = HotelDetailsHelper3(obj.packages[obj.selectedPack].hotel.originalLink, obj.selectedPack);

            // ---------------------- load reviews
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
            // -------------------------------------


            // get image by GoogleAPI
            ImageSearch image = new ImageSearch();
            try
            {
                image.SetHotelName(results.property_name.ToString()); // initialize object by GoogleAPI
                image.SavePhotoReferences();
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                return ViewPackageInfo(obj);
            }

            List<string> imageLST = image.GetImages();

            HotelOrderDetailsVM hotel = new HotelOrderDetailsVM()
            {
                hotelName = results.property_name,
                address = "ADDRESS: " + results.address.line1 + ", " + results.address.city,
                contact = contactLST,
                amenities = amenitiesLST,
                rating = obj.packages[obj.selectedPack].hotel.rating.ToString(),
                latitude = results.location.latitude,
                longitude = results.location.longitude,
                room = roomLST,
                image = imageLST,
                apiKey = image.GetKey(),
                reviews = hotelReviewsList,
                customersName = customersName,
            };

            obj.packages[obj.selectedPack].flight.timeDuration_outbound = ToolsClass.TimeDifferences(obj.packages[obj.selectedPack].flight.outbound.DepartureTime, obj.packages[obj.selectedPack].flight.outbound.ArrivalTime);
            obj.packages[obj.selectedPack].flight.timeDuration_inbound = ToolsClass.TimeDifferences(obj.packages[obj.selectedPack].flight.inbound.DepartureTime, obj.packages[obj.selectedPack].flight.inbound.ArrivalTime);

            FlightInfoVM flight = obj.packages[obj.selectedPack].flight;


            PackageInfoVM package = new PackageInfoVM()
            {
                hotel = hotel,
                flight = flight,
                nights = obj.nights,
                price = obj.packages[obj.selectedPack].price,
                composition = obj.composition,
            };

            return View(package);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult ViewPackageInfo()
        {
            List<string> amenities = new List<string>();
            amenities.Add("Elevators");
            amenities.Add("Sauna");
            amenities.Add("Room service");
            amenities.Add("Internet services");
            amenities.Add("220 AC");
            amenities.Add("Parking");
            amenities.Add("Pets allowed");
            amenities.Add("Wireless internet connection in public areas");
            amenities.Add("Lounges/bars");
            amenities.Add("24-hour front desk");

            List<string> contacts = new List<string>();
            contacts.Add("PHONE: 49/8161/5320");
            contacts.Add("FAX: 49/8161/532100");
            contacts.Add("EMAIL: HA0Q8@accor.com");
            contacts.Add("URL: www.mercure.com");

            List<string> images = new List<string>();
            images.Add("https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&photoreference=CmRaAAAAtIARwdNqhiQNC-jj-6upRvxxBXgk5Wdl_t484HkmD_ecnJrvi1wK3l53njD6C-XYc0dUEdEGg_SThMQJs7bZpb-2SpTAIUbRmTPoe2aOnpK7tIeC-oERWKsSjvS3H7eyEhA1a_4sr0mjoc7yFPpqvEFhGhSzrFsXmCqtC_7EZdfsvH1zdCjqEQ&key=AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98");
            images.Add("https://maps.googleapis.com/maps/api/place/photo?maxwidth=800&photoreference=CmRaAAAA9zNoq-JwlWS9MkZCsM0FxyrXoogblHtJtoaPeJRzD2DZVWQS5qrq9vXYY7gCqS-MrEAd389xmp57gKYv7k1KzeqN7pO2Ufc1EUs6IxTX6D2541mMA-N5cAIVxf7ib_nbEhAoYFrmfqLRbhQbzIkNGqh3GhTk2uP9_DRnbDVmt_Nihbz0_ZDbIA&key=AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98");

            List<string> roomType1 = new List<string>();
            roomType1.Add("Room type: Unspecified");
            roomType1.Add("Bed type: Double");
            roomType1.Add("Number of beds: 1");
            List<string> roomType2 = new List<string>();
            roomType2.Add("Room type: Unspecified");
            roomType2.Add("Bed type: Twin");
            roomType2.Add("Number of beds: 2");

            List<RoomVM> rooms = new List<RoomVM>();
            rooms.Add(new RoomVM()
            {
                roomDescription = "1 DOUBLE BED NONSMOKING FEEL YOURSELF AT HOME ",
                roomPrice = 684.27,
                roomType = roomType1
            });
            rooms.Add(new RoomVM()
            {
                roomDescription = "2 SINGLE BEDS NONSMOKING FEEL YOURSELF AT HOME ",
                roomPrice = 684.27,
                roomType = roomType1
            });

            HotelOrderDetailsVM hotel = new HotelOrderDetailsVM()
            {
                address = "ADDRESS: Dr Von Daller Str 1 3, Freising",
                amenities = amenities,
                apiKey = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98",
                contact = contacts,
                hotelName = "Mercure Muc Freising Airport",
                image = images,
                latitude = 48.40068,
                longitude = 11.75076,
                rating = "4",
                room = rooms,
                reviews = new List<HotelReview>(),
                customersName = new List<string>(),                
            };

            FlightRouteVM inbound = new FlightRouteVM()
            {
                ArrivalAirportCode = "TLV",
                ArrivalAirportName = "Ben Gurion Airport",
                ArrivalDate = "18.09.2018",
                ArrivalDay = "Tuesday",
                ArrivalTime = "03:35",
                DepartureAirportCode = "MUC",
                DepartureAirportName = "Munich International Airport",
                DepartureDate = "17.09.2018",
                DepartureDay = "Monday",
                DepartureTime = "23:00",
                FlightNumber = "680"
            };

            FlightRouteVM outbound = new FlightRouteVM()
            {
                ArrivalAirportCode = "MUC",
                ArrivalAirportName = "Munich International Airport",
                ArrivalDate = "10.09.2018",
                ArrivalDay = "Monday",
                ArrivalTime = "09:55",
                DepartureAirportCode = "TLV",
                DepartureAirportName = "Ben Gurion Airport",
                DepartureDate = "10.09.2018",
                DepartureDay = "Monday",
                DepartureTime = "06:55",
                FlightNumber = "681"
            };

            FlightInfoVM flight = new FlightInfoVM()
            {
                airline = "LH",
                departureDate = "10.09.2018",
                destinationCity = "Munich",
                destinationCodeCity = "MUC",
                destinationCountry = "Germany",
                inbound = inbound,
                originCity = "Tel Aviv",
                originCodeCity = "TLV",
                originCountry = "Israel",
                outbound = outbound,
                price = "555.07",
                returnDate = "17.09.2018",
                seatsRemaining = "9",
                timeDuration_outbound = ToolsClass.TimeDifferences("06:55", "09:55"),
                timeDuration_inbound = ToolsClass.TimeDifferences("23:00", "03:35"),
            };

            PackageInfoVM package = new PackageInfoVM()
            {
                hotel = hotel,
                flight = flight,
                nights = 7,
                price = 3121.4,
                composition = "Couple",
            };

            return View(package);
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
        public ActionResult SubmitPackage(PackageInfoVM obj)
        {
            int numberOfTickets = CalculateNumberOfTickets(obj.composition);

            PackageCheckoutVM objToPass = new PackageCheckoutVM()
            {
                destCountry = obj.flight.destinationCountry,
                hotelName = obj.hotel.hotelName,
                numberOfRooms = obj.hotel.numberOfRooms,
                roomDescription = obj.hotel.room[0].roomDescription,
                composition = obj.composition,
                departureFlightNumber = obj.flight.outbound.FlightNumber,
                returnFlightNumber = obj.flight.inbound.FlightNumber,
                departureDate = obj.flight.departureDate,
                returnDate = obj.flight.returnDate,
                departureLocation = obj.flight.originCity,
                returnLocation = obj.flight.destinationCity,
                numberOfTickets = numberOfTickets,
                airline = obj.flight.airline,
                totalFee = obj.price.ToString(),
            };

            return View("PackageCheckout", objToPass);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult PackageCheckout()
        {
            PackageCheckoutVM objToPass = new PackageCheckoutVM()
            {
                hotelName = "Mercure Muc Freising Airport",
                numberOfRooms = 2,
                roomDescription = "1 DOUBLE BED NONSMOKING FEEL YOURSELF AT HOME",
                composition = "Couple + 3 Children",
                departureFlightNumber = "681",
                returnFlightNumber = "680",
                departureDate = "10.09.2018",
                returnDate = "17.09.2018",
                departureLocation = "Tel Aviv",
                returnLocation = "Munich",
                numberOfTickets = 5,
                airline = "LH",
                totalFee = "7592.32",
            };

            return View(objToPass);
        }

        private int CalculateNumberOfTickets(string composition)
        {
            switch (composition)
            {
                case "Couple":
                    return 2;
                case "Single":
                    return 1;
                case "Adult + Child":
                    return 2;
                case "Adult + 2 Children":
                    return 3;
                case "Adult + 3 Children":
                    return 4;
                case "Couple + Child":
                    return 3;
                case "Couple + 2 Children":
                    return 4;
                case "Couple + 3 Children":
                    return 5;
                case "3 Adults":
                    return 3;
                default:
                    return 0;
            }
        }
        
        [HttpPost]
        public ActionResult SubmitOrder(PackageCheckoutVM obj)
        {
            DataLayer dl = new DataLayer();

            DateTime today = DateTime.Today;
            string date = today.ToString("dd/MM/yyyy").Replace("/", ".");
            double totalFee = Convert.ToDouble(obj.totalFee.Replace("$", ""));
            bool sameCustomer = false;

            // first check if customer already exists in table
            List<Customer> customerFromDB = (from u in dl.customers
                                             where u.Email.ToLower() == obj.email.ToLower()
                                             select u).ToList<Customer>();

            if (customerFromDB.Count != 0) // if customer already exists
            {
                // check if the input is the same as the customer data
                if (customerFromDB[0].FullName != obj.customerName || customerFromDB[0].ID != obj.id)
                {
                    TempData["error"] = "This email address already exist in our records with different name and/or different id!";
                    return View("PackageCheckout", obj);
                }
                else if (customerFromDB[0].Location != obj.address || customerFromDB[0].PhoneNumber != obj.phone)
                {
                    // update: delete this row and later add new customer row
                    dl.customers.Remove(customerFromDB[0]);
                    dl.SaveChanges();
                }
                // else: same row - no need to update
                else
                    sameCustomer = true;
            }

            // --------------------- create object and save to table 1: Order
            Order order = new Order()
            {
                CreditCard = obj.creditCard,
                CustomerEmail = obj.email,
                OrderDate = date,
                Status = 0,
                TotalPayment = totalFee,
                Type = "Package",
                OrderDestination = obj.destCountry,
            };

            dl.orders.Add(order); // adding in memory and not to DB //
            dl.SaveChanges();
            int INVOICE = order.InvoiceNumber;

            // --------------------- create object and save to table 2: PackageOrder
            PackageOrder PackageOrder = new PackageOrder()
            {
                InvoiceID = INVOICE,
                Composition = obj.composition,
                DepartureLocation = obj.departureLocation,
                ArrivalLocation = obj.returnLocation,
                DepartureDate = obj.departureDate,
                ReturnDate = obj.returnDate,
                Outbound_FlightNumber = int.Parse(obj.departureFlightNumber),
                Inbound_FlightNumber = int.Parse(obj.returnFlightNumber),
                Airline = obj.airline,
                NumberOfTickets = obj.numberOfTickets,
                HotelName = obj.hotelName,
                NumberOfRooms = obj.numberOfRooms,
                RoomDescription = obj.roomDescription,
                Reviewed=0
            };

            dl.packageOrders.Add(PackageOrder); // adding in memory and not to DB //

            // --------------------- create object and save to table 3: Customer
            if (!sameCustomer)
            {
                Customer customer = new Customer()
                {
                    Email = obj.email,
                    FullName = obj.customerName,
                    Location = obj.location,
                    ID = obj.id,
                    PhoneNumber = obj.phone
                };

                dl.customers.Add(customer); // adding in memory and not to DB //
            }

            dl.SaveChanges();

            // go to recipt
            return RedirectToAction("ViewOrderReceipt", "Support", new { invoice = INVOICE });
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}