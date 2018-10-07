using Click4Trip.APIs;
using Click4Trip.Classes;
using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.Tools;
using Click4Trip.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Click4Trip.Controllers
{
    public class FlightController : Controller
    {
        //                                               Index page for Flights (include search)
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Index() => View(LoadCities());

        private FlightIndexVM LoadCities()
        {
            FlightIndexVM fivm = new FlightIndexVM();
            DataLayer dl = new DataLayer();

            fivm.originCountries = (from u in dl.locations
                                    select u.country).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            fivm.originCities = (from u in dl.locations
                                 where u.country == fivm.originCountries.FirstOrDefault()
                                 select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            fivm.desCountries = new List<string>(fivm.originCountries);
            fivm.desCities = new List<string>(fivm.originCities);

            return fivm;
        }

        [HttpPost]
        public ActionResult SubmitSearchGeneral(GeneralIndexVM obj)
        {
            FlightIndexVM flight = new FlightIndexVM()
            {
                desCity = obj.desCity,
                desCountry = obj.desCountry,
                originCity = obj.originCity,
                originCountry = obj.originCountry,
                sdate = obj.startDate,
                edate = obj.endDate,
            };

            return SubmitSearch(flight);
        }

        [HttpPost]
        public ActionResult SubmitSearch(FlightIndexVM flightsData)
        {
            // convert origin and destination cities to codes
            DataLayer dl = new DataLayer();
            flightsData.originCode = (from u in dl.locations
                               where u.city.ToLower() == flightsData.originCity.ToLower()
                               select u.code).ToList<string>().FirstOrDefault();

            flightsData.desCode = (from u in dl.locations
                                      where u.city.ToLower() == flightsData.desCity.ToLower()
                                      select u.code).ToList<string>().FirstOrDefault();

            // create the api object
            FlightSearch flightSearch = new FlightSearch();
            flightSearch.FillData(flightsData.originCode, flightsData.desCode, flightsData.sdate, flightsData.edate, flightsData.originCountry, flightsData.originCity, flightsData.desCountry, flightsData.desCity);
            
            // if no results were found (400: bad request)
            if (flightSearch.GetResponse() != "")
            {
                FlightSearchResultsVM newObj = new FlightSearchResultsVM()
                {
                    destinationCity = flightSearch.GetDestinationCity(),
                    destinationCountry = flightSearch.GetDestinationCountry(),
                    originCity = flightSearch.GetOriginCity(),
                    originCountry = flightSearch.GetOriginCountry(),
                    departureDate = flightsData.sdate,
                    returnDate = flightsData.edate,
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
                latitude = latitude,
                longitude = longitude,
            };

            return View("SearchResults",flightSearchResultsVM);
        }

        // Temp ActionResult
        public ActionResult SearchResults()
        {
            List<string> airline = new List<string>();
            airline.Add("U2");
            airline.Add("IZ");
            airline.Add("BS");

            List<FlightRouteVM> inbound = new List<FlightRouteVM>();
            FlightRouteVM fr = new FlightRouteVM()
            {
                ArrivalAirportCode = "TLV",
                ArrivalAirportName = "Ben Gurion Airport",
                ArrivalDate = "10.09.2018",
                ArrivalDay = "Monday",
                ArrivalTime = "14:15",
                DepartureAirportCode = "LTN",
                DepartureAirportName = "Luton Airport",
                DepartureDate = "10.09.2018",
                DepartureDay = "Monday",
                DepartureTime = "07:15",
                FlightNumber = "2083",
                TimeDuration = ToolsClass.TimeDifferences("07:15", "14:15"),
            };

            inbound.Add(fr);
            inbound.Add(fr);
            inbound.Add(fr);

            List<FlightRouteVM> outbound = new List<FlightRouteVM>();
            FlightRouteVM fr2 = new FlightRouteVM()
            {
                ArrivalAirportCode = "LTN",
                ArrivalAirportName = "Luton Airport",
                ArrivalDate = "03.09.2018",
                ArrivalDay = "Monday",
                ArrivalTime = "23:50",
                DepartureAirportCode = "TLV",
                DepartureAirportName = "Ben Gurion Airport",
                DepartureDate = "03.09.2018",
                DepartureDay = "Monday",
                DepartureTime = "20:25",
                FlightNumber = "2086",
                TimeDuration = ToolsClass.TimeDifferences("20:25", "23:50"),
            };

            outbound.Add(fr2);
            outbound.Add(fr2);
            outbound.Add(fr2);

            List<double> price = new List<double>();
            price.Add(404.35);
            price.Add(468.94);
            price.Add(1203.94);

            List<string> seatsRemaining = new List<string>();
            seatsRemaining.Add("9");
            seatsRemaining.Add("9");
            seatsRemaining.Add("9");

            FlightSearchResultsVM objToPass = new FlightSearchResultsVM()
            {
                departureDate = "03.09.2018",
                destinationCity = "London",
                destinationCodeCity = "LON",
                destinationCountry = "Uk",
                inbound = inbound,
                originCity = "Tel aviv",
                originCodeCity = "TLV",
                originCountry = "Israel",
                outbound = outbound,
                price = price,
                returnDate = "10.09.2018",
                seatsRemaining = seatsRemaining,
                airline = airline,
                latitude = 51.5,
                longitude = -0.1667,
            };
            return View(objToPass);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               View flight
        //-----------------------------------------------------------------------------------------------------------------------------
        private FlightInfoVM LoadDataFor_ViewFlightInfo(FlightSearchResultsVM flightSearchResultsVM)
        {
            int index = flightSearchResultsVM.selectedFlight;
            FlightInfoVM flightInfoVM = new FlightInfoVM()
            {
                airline = flightSearchResultsVM.airline[index],
                departureDate = flightSearchResultsVM.departureDate,
                seatsRemaining = flightSearchResultsVM.seatsRemaining[index],
                returnDate = flightSearchResultsVM.returnDate,
                destinationCity = flightSearchResultsVM.destinationCity,
                destinationCodeCity = flightSearchResultsVM.destinationCodeCity,
                destinationCountry = flightSearchResultsVM.destinationCountry,
                inbound = flightSearchResultsVM.inbound[index],
                originCity = flightSearchResultsVM.originCity,
                originCodeCity = flightSearchResultsVM.originCodeCity,
                originCountry = flightSearchResultsVM.originCountry,
                outbound = flightSearchResultsVM.outbound[index],
                price = flightSearchResultsVM.price[index].ToString(),
                timeDuration_outbound = ToolsClass.TimeDifferences(flightSearchResultsVM.outbound[index].DepartureTime, flightSearchResultsVM.outbound[index].ArrivalTime),
                timeDuration_inbound = ToolsClass.TimeDifferences(flightSearchResultsVM.inbound[index].DepartureTime, flightSearchResultsVM.inbound[index].ArrivalTime),              
            };
            return flightInfoVM;
        }

        [HttpPost]
        public ActionResult ViewFlightInfo(FlightSearchResultsVM flightSearchResultsVM) => View(LoadDataFor_ViewFlightInfo(flightSearchResultsVM));

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult ViewFlightInfo()
        {
            List<string> airlineLST = new List<string>();
            airlineLST.Add("LY");
            List<double> priceLST = new List<double>();
            priceLST.Add(2565.42);
            List<string> seatsRemainingLST = new List<string>();
            seatsRemainingLST.Add("9");

            List<FlightRouteVM> outboundLST = new List<FlightRouteVM>();
            outboundLST.Add(new FlightRouteVM() {
                ArrivalAirportCode = "LHR",
                ArrivalDate = "15.06.2018",
                ArrivalTime = "13:35",
                DepartureAirportCode = "TLV",
                DepartureDate = "15.06.2018",
                DepartureTime = "10:10",
                FlightNumber = "315",
                ArrivalAirportName = "London Heathrow Airport",
                DepartureAirportName = "Ben Gurion Airport",
                DepartureDay = ToolsClass.DayOfTheWeek("15.06.2018"),
                ArrivalDay = ToolsClass.DayOfTheWeek("15.06.2018"),
            });

            List<FlightRouteVM> inboundLST = new List<FlightRouteVM>();
            inboundLST.Add(new FlightRouteVM()
            {
                ArrivalAirportCode = "TLV",
                ArrivalDate = "25.06.2018",
                ArrivalTime = "15:00",
                DepartureAirportCode = "LHR",
                DepartureDate = "25.06.2018",
                DepartureTime = "08:10",
                FlightNumber = "165",
                ArrivalAirportName = "Ben Gurion Airport",
                DepartureAirportName = "London Heathrow Airport",
                DepartureDay = ToolsClass.DayOfTheWeek("25.06.2018"),
                ArrivalDay = ToolsClass.DayOfTheWeek("25.06.2018"),
            });

            FlightSearchResultsVM flightSearchResultsVM = new FlightSearchResultsVM()
            {
                selectedFlight = 0,
                departureDate = "15.06.2018",
                returnDate = "25.06.2018",
                originCountry = "UK",
                originCity = "London",
                destinationCountry = "Israel",
                destinationCity = "Tel Aviv",
                originCodeCity = "LON",
                destinationCodeCity = "TLV",
                outbound = outboundLST,
                inbound = inboundLST,
                airline = airlineLST,
                price = priceLST,
                seatsRemaining = seatsRemainingLST
            };

            return View(LoadDataFor_ViewFlightInfo(flightSearchResultsVM));
        }

        [HttpPost]
        public ActionResult SubmitFlightComposition(FlightInfoVM obj)
        {
            string[] temp_composition = obj.composition.Split(',');
            int size = temp_composition.Length;

            int[] composition = new int[size];
            string[] composition_price = new string[size];
            for (int i = 0; i < size; ++i)
            {
                composition[i] = int.Parse(temp_composition[i]);
            }

            composition_price[0] = (Math.Round(double.Parse(obj.price) * 0.4, 2)).ToString();
            composition_price[1] = (Math.Round(double.Parse(obj.price) * 0.6, 2)).ToString();
            composition_price[2] = (Math.Round(double.Parse(obj.price) * 1.2, 2)).ToString();
            composition_price[3] = (Math.Round(double.Parse(obj.price) * 1, 2)).ToString();
            composition_price[4] = (Math.Round(double.Parse(obj.price) * 0.8, 2)).ToString();

            FlightCheckoutVM objToPass = new FlightCheckoutVM()
            {
                destCountry = obj.destinationCountry,
                departureFlightNumber = obj.outbound.FlightNumber,
                returnFlightNumber = obj.inbound.FlightNumber,
                departureDate = obj.departureDate,
                returnDate = obj.returnDate,
                departureLocation = obj.originCity,
                returnLocation = obj.destinationCity,
                numberOfTickets = composition[0] + composition[1] + composition[2] + composition[3] + composition[4],
                babies = composition[0],
                children = composition[1],
                youngsters = composition[2],
                adults = composition[3],
                pensioners = composition[4],
                price_babies = composition_price[0],
                price_children = composition_price[1],
                price_youngsters = composition_price[2],
                price_adults = composition_price[3],
                price_pensioners = composition_price[4],
                airline = obj.airline,
                totalFee = obj.totalPrice,
            };

            return View("FlightCheckout", objToPass);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult FlightCheckout()
        {
            FlightCheckoutVM objToPass = new FlightCheckoutVM()
            {
                departureFlightNumber = "150",
                returnFlightNumber = "151",
                departureDate = "15.06.2018",
                returnDate = "25.06.2018",
                departureLocation = "Tel Aviv",
                returnLocation = "London",
                numberOfTickets = 5,
                babies = 1,
                children = 1,
                youngsters = 1,
                adults = 2,
                pensioners = 0,
                price_babies = "40.00",
                price_children = "60.00",
                price_youngsters = "120.00",
                price_adults = "100.00",
                price_pensioners = "90.00",
                airline = "LY",
                totalFee = "420.00",
            };

            return View(objToPass);
        }

        [HttpPost]
        public ActionResult SubmitOrder(FlightCheckoutVM obj)
        {
            DataLayer dl = new DataLayer();

            DateTime today = DateTime.Today;
            string date = today.ToString("dd/MM/yyyy").Replace("/", ".");
            double totalFee = Convert.ToDouble(obj.totalFee.Replace("$",""));
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
                    return View ("FlightCheckout", obj);
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
                Type = "Flight",
                OrderDestination = obj.destCountry,
            };

            dl.orders.Add(order); // adding in memory and not to DB //
            dl.SaveChanges();
            int INVOICE = order.InvoiceNumber;

            // --------------------- create object and save to table 2: FlightOrder
            FlightOrder flightOrder = new FlightOrder()
            {
                InvoiceID = INVOICE,
                DepartureLocation = obj.departureLocation,
                ArrivalLocation = obj.returnLocation,
                DepartureDate = obj.departureDate,
                ReturnDate = obj.returnDate,
                Outbound_FlightNumber = int.Parse(obj.departureFlightNumber),
                Inbound_FlightNumber = int.Parse(obj.returnFlightNumber),
                Airline = obj.airline,
                NumberOfBabies = obj.babies,
                NumberOfChildren = obj.children,
                NumberOfYoungsters = obj.youngsters,
                NumberOfAdults = obj.adults,
                NumberOfPensioners = obj.pensioners,
                PricePerBaby = double.Parse(obj.price_babies),
                PricePerChild = double.Parse(obj.price_children),
                PricePerYoungster = double.Parse(obj.price_youngsters),
                PricePerAdult = double.Parse(obj.price_adults),
                PricePerPensioner = double.Parse(obj.price_pensioners),
            };

            dl.flightOrders.Add(flightOrder); // adding in memory and not to DB //

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