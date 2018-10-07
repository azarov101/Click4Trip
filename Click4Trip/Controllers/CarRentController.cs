using Click4Trip.APIs;
using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Click4Trip.Controllers
{
    public class CarRentController : Controller
    {
        //                                               Index page for Car Rent (include search)
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult Index() => View(LoadCities());

        private CarRentIndexVM LoadCities()
        {
            CarRentIndexVM fivm = new CarRentIndexVM();
            DataLayer dl = new DataLayer();

            fivm.desCountries = (from u in dl.locations
                                 select u.country).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            fivm.desCities = (from u in dl.locations
                              where u.country == fivm.desCountries.FirstOrDefault()
                              select u.city).ToList<string>().GroupBy(p => p)
                                  .Select(g => g.First()).OrderBy(q => q)
                                  .ToList();

            return fivm;
        }

        [HttpPost]
        public ActionResult SubmitSearchGeneral(GeneralIndexVM obj)
        {
            CarRentIndexVM car = new CarRentIndexVM()
            {
                desCity = obj.desCity,
                desCountry = obj.desCountry,
                sdate = obj.startDate,
                edate = obj.endDate,
            };

            return SubmitSearch(car);
        }

        [HttpPost]
        public ActionResult SubmitSearch(CarRentIndexVM carsData)
        {
            DataLayer dl = new DataLayer();

            carsData.desCode = (from u in dl.locations
                                where u.city.ToLower() == carsData.desCity.ToLower()
                                select u.code).ToList<string>().FirstOrDefault();

            // create a list of cars with api
            CarRentSearch crs = new CarRentSearch();
            crs.FillData(carsData.desCode, carsData.sdate, carsData.edate);
            
            CarRentSearchResultsVM carRentSearchResultsVM = new CarRentSearchResultsVM()
            {
                latitude = crs.GetLatitude(),
                longitude = crs.GetLongitude(),
                PickupDate = crs.GetPickupDate(),
                DropoffDate = crs.GetDropoffDate(),
                LocationCodeCity = crs.GetLocationCodeCity(),
                provider = crs.GetProvider(),
                branch_id = crs.GetBranch(),
                airport = crs.GetAirport(),
                address = crs.GetAddress(),
                cars = crs.GetCars(),
            };

            CarRentViewVM cr = new CarRentViewVM(carRentSearchResultsVM);
            cr.Country = carsData.desCountry;
            cr.City = carsData.desCity;

            // for the google map
            string realCode = crs.GetLocation(carsData.desCode, "firstStage");
            string[] loc = crs.GetLocation(realCode, "secondStage").Split(',');
            cr.latitude = Double.Parse(loc[0]);
            cr.longitude = Double.Parse(loc[1]);

            return View("SearchResults", cr);
        }

        // Temp ActionResult for static run
        public ActionResult SearchResults()
        {
            CarRentIndexVM obj = new CarRentIndexVM()
            {
                desCity = "Tel aviv",
                desCountry = "Israel",
                edate = "19.06.2018",
                sdate = "18.06.2018",
            };

            return SubmitSearch(obj);
        }
        //-----------------------------------------------------------------------------------------------------------------------------


        //                                               View Car Information
        //-----------------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult ViewCarInfo(CarRentViewVM obj)
        {
            CarInfoVM objToPass = new CarInfoVM(obj);
            return View(objToPass);
        }

        // ----------------------------- this view is only for static run (TEMP VIEW)
        public ActionResult ViewCarInfo()
        {
            CarInfoVM objToPass = new CarInfoVM();
            return View(objToPass);
        }
        //-----------------------------------------------------------------------------------------------------------------------------


        //                                               Car Rent Payment Page (include save to DB)
        //-----------------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult SubmitCar(CarInfoVM obj)
        {
            CarRentCheckoutVM objToPass = new CarRentCheckoutVM(obj);
            return View("CarRentCheckout", objToPass);
        }

        [HttpPost]
        public ActionResult SubmitOrder(CarRentCheckoutVM obj)
        {
            DataLayer dl = new DataLayer();

            DateTime today = DateTime.Today;
            string date = today.ToString("dd/MM/yyyy").Replace("/", ".");
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
                TotalPayment = obj.price,
                Type = "CarRent",
                OrderDestination = obj.destCountry,
            };

            dl.orders.Add(order); // adding in memory and not to DB //
            dl.SaveChanges();
            int INVOICE = order.InvoiceNumber;

            // --------------------- create object and save to table 2: RentCarOrder
            RentCarOrder rentCarOrder = new RentCarOrder()
            {
                InvoiceID = INVOICE,
                AcrissCode = obj.acriss_code,
                Address = obj.address,
                Days = Convert.ToInt32(obj.days),
                DropOffDate = obj.dropOffDate,
                PickUpDate = obj.pickUpDate,
                Price = obj.price,
                Provider = obj.provider,
            };

            dl.rentCarOrder.Add(rentCarOrder); // adding in memory and not to DB //

            // --------------------- create object and save to table 3: Customer
            if (!sameCustomer)
            {
                Customer customer = new Customer()
                {
                    Email = obj.email,
                    FullName = obj.customerName,
                    Location = "Israel", // benya's static function
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