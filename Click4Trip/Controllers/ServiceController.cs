using Click4Trip.Classes;
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
    [SalesManAuthorization]
    public class ServiceController : Controller
    {
        //                                               View Order
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewOrder() => View();

        [HttpPost]
        public ActionResult SubmitVerification(IdentifyVM ivm)
        {
            DataLayer dl = new DataLayer();
            Order order = (from x in dl.orders
                           where x.InvoiceNumber == ivm.InvoiceId
                           select x).ToList<Order>().FirstOrDefault();

            if (order == null)
            {
                ViewData["msg"] = "Invalid Id and/or Invoice!";
                return View("ViewOrder");
            }

            // IN CASE OF HOTEL
            if (order.Type == "Hotel")
                return View("ViewHotelOrderReceipt",returnHotel(order));

            // IN CASE OF FLIGHT
            if (order.Type == "Flight")
                return View("ViewFlightOrderReceipt", returnFlight(order));

            // IN CASE OF PACKAGE
            if (order.Type == "Package")
                return View("ViewPackageOrderReceipt", returnPackage(order));

            // IN CASE OF RENTCAR
            if (order.Type == "CarRent")
                return View("ViewCarRentOrderReceipt", returnCarRent(order));

            return View();
        }

        public static ViewHotelOrderVM returnHotel(Order order)
        {
            ViewHotelOrderVM ovm = new ViewHotelOrderVM();

            DataLayer dl = new DataLayer();

            ovm.invoice = order.InvoiceNumber;

            ovm.creditCardNumber = order.CreditCard;

            HotelOrder hotelorder = (from x in dl.hotelOrders
                                     where x.InvoiceID == ovm.invoice
                                     select x).ToList<HotelOrder>().FirstOrDefault();

            ovm.hotelName = hotelorder.HotelName;

            ovm.startDate = hotelorder.StartDate;

            ovm.endDate = hotelorder.EndDate;

            ovm.orderDate = order.OrderDate;

            ovm.status = order.Status;

            ovm.email = order.CustomerEmail;

            Customer cust = (from x in dl.customers
                             where x.Email == ovm.email
                             select x).ToList<Customer>().FirstOrDefault();

            ovm.customerName = cust.FullName;

            ovm.customerId = cust.ID;

            List<HotelOrderDetails> hod = (from x in dl.hotelOrderDetails
                                           where x.Invoice == ovm.invoice
                                           select x).ToList<HotelOrderDetails>();

            ovm.totalPrice = order.TotalPayment;

            //ovm.rooms = null;
            ovm.numberOfRooms = hod.Count;

            List<RoomVM> rooms = new List<RoomVM>();
            for (int i = 0; i < hod.Count; ++i)
            {
                RoomVM r = new RoomVM()
                {
                    roomComposition = hod[i].RoomComposition,
                    roomPrice = hod[i].PaymentForRoom,
                    roomDescription = hod[i].RoomType
                };
                rooms.Add(r);
            }


            ovm.rooms = rooms;
            return ovm;
        }

        public static ViewFlightOrderVM returnFlight(Order order)
        {
            ViewFlightOrderVM ovm = new ViewFlightOrderVM();

            DataLayer dl = new DataLayer();

            ovm.invoice = order.InvoiceNumber;

            ovm.creditCardNumber = order.CreditCard;

            FlightOrder flightorder = (from x in dl.flightOrders
                                       where x.InvoiceID == ovm.invoice
                                       select x).ToList<FlightOrder>().FirstOrDefault();

            ovm.ArrivalLocation = flightorder.ArrivalLocation;

            ovm.DetartureLocation = flightorder.DepartureLocation;

            ovm.DetartureDate = flightorder.DepartureDate;

            ovm.ReturnDate = flightorder.ReturnDate;

            ovm.orderDate = order.OrderDate;

            ovm.status = order.Status;

            ovm.email = order.CustomerEmail;

            ovm.outbound = flightorder.Outbound_FlightNumber;

            ovm.inbound = flightorder.Inbound_FlightNumber;

            Customer cust = (from x in dl.customers
                             where x.Email == ovm.email
                             select x).ToList<Customer>().FirstOrDefault();

            ovm.customerName = cust.FullName;

            ovm.customerId = cust.ID;

            ovm.totalPrice = order.TotalPayment;

            ovm.NumBabies = flightorder.NumberOfBabies;
            ovm.NumChild = flightorder.NumberOfChildren;
            ovm.NumYoung = flightorder.NumberOfYoungsters;
            ovm.NumAdult = flightorder.NumberOfAdults;
            ovm.NumPensioner = flightorder.NumberOfPensioners;

            ovm.PriceBabies = flightorder.PricePerBaby;
            ovm.PriceChild = flightorder.PricePerChild;
            ovm.PriceYoung = flightorder.PricePerYoungster;
            ovm.PriceAdult = flightorder.PricePerAdult;
            ovm.PricePensioner = flightorder.PricePerPensioner;

            ovm.Airline = flightorder.Airline;


            return ovm;
        }

        public static ViewPackageOrderVM returnPackage(Order order)
        {
            ViewPackageOrderVM ovm = new ViewPackageOrderVM();

            DataLayer dl = new DataLayer();

            ovm.invoice = order.InvoiceNumber;

            ovm.creditCardNumber = order.CreditCard;

            PackageOrder packageorder = (from x in dl.packageOrders
                                         where x.InvoiceID == ovm.invoice
                                         select x).ToList<PackageOrder>().FirstOrDefault();

            ovm.orderDate = order.OrderDate;

            ovm.status = order.Status;

            ovm.email = order.CustomerEmail;

            ovm.ArrivalLocation = packageorder.ArrivalLocation;

            ovm.DetartureLocation = packageorder.DepartureLocation;

            ovm.DetartureDate = packageorder.DepartureDate;

            ovm.ReturnDate = packageorder.ReturnDate;

            ovm.outbound = packageorder.Outbound_FlightNumber;

            ovm.inbound = packageorder.Inbound_FlightNumber;

            ovm.hotelName = packageorder.HotelName;

            ovm.composition = packageorder.Composition;
            Customer cust = (from x in dl.customers
                             where x.Email == ovm.email
                             select x).ToList<Customer>().FirstOrDefault();

            ovm.customerName = cust.FullName;

            ovm.customerId = cust.ID;
            ovm.roomdescription = packageorder.RoomDescription;
            ovm.numberOfRooms = packageorder.NumberOfRooms;
            ovm.totalPrice = order.TotalPayment;
            ovm.numTickets = packageorder.NumberOfTickets;
            ovm.Airline = packageorder.Airline;
            return ovm;
        }

        public static ViewCarRentVM returnCarRent(Order order)
        {
            ViewCarRentVM ovm = new ViewCarRentVM();

            DataLayer dl = new DataLayer();

            ovm.invoice = order.InvoiceNumber;

            ovm.creditCardNumber = order.CreditCard;

            RentCarOrder carrentorder = (from x in dl.rentCarOrder
                                         where x.InvoiceID == ovm.invoice
                                         select x).ToList<RentCarOrder>().FirstOrDefault();

            ovm.orderDate = order.OrderDate;

            ovm.status = order.Status;

            ovm.email = order.CustomerEmail;

            ovm.PickUpDate = carrentorder.PickUpDate;

            ovm.DropOffDate = carrentorder.DropOffDate;

            ovm.days = carrentorder.Days;

            ovm.address = carrentorder.Address;

            ovm.provider = carrentorder.Provider;

            ovm.Price = carrentorder.Price;

            ovm.acrisscode = carrentorder.AcrissCode;

            Customer cust = (from x in dl.customers
                             where x.Email == ovm.email
                             select x).ToList<Customer>().FirstOrDefault();

            ovm.customerName = cust.FullName;

            ovm.customerId = cust.ID;
            ovm.totalPrice = order.TotalPayment;
            return ovm;
        }

        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Order Confirmation
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult OrderConfirmation()
        {
            DataLayer dl = new DataLayer();
            OrderConfirmVM ocvm = new OrderConfirmVM();
            List<Order> hotelOrders = (from x in dl.orders
                                        where x.Status == 0 && x.Type == "Hotel"
                                        select x).ToList<Order>();

            foreach (Order o in hotelOrders)
                ocvm.hotelOrderVM.Add(returnHotel(o));

            List<Order> flightOrders = (from x in dl.orders
                                       where x.Status == 0 && x.Type == "Flight"
                                       select x).ToList<Order>();

            foreach (Order o in flightOrders)
                ocvm.flightOrderVM.Add(returnFlight(o));

            List<Order> packageOrders = (from x in dl.orders
                                             where x.Status == 0 && x.Type == "Package"
                                             select x).ToList<Order>();

            foreach (Order o in packageOrders)
                ocvm.packageOrderVM.Add(returnPackage(o));

            List<Order> carRentOrders = (from x in dl.orders
                                         where x.Status == 0 && x.Type == "CarRent"
                                         select x).ToList<Order>();

            foreach (Order o in carRentOrders)
                ocvm.carRentOrderVM.Add(returnCarRent(o));


            return View(ocvm);
        }

        public ActionResult ConfirmHotelOrderByJSON(int id)
        {
            DataLayer dl = new DataLayer();
            List<Order> orderlist = (from o in dl.orders
                                        where o.InvoiceNumber == id
                                        select o).ToList<Order>();

            foreach (Order x in orderlist)
                x.Status = 1;
            dl.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeclineHotelOrderByJSON(int id)
        {
            DataLayer dl = new DataLayer();
            List<Order> orderlist = (from o in dl.orders
                                        where o.InvoiceNumber == id
                                        select o).ToList<Order>();

            foreach (Order x in orderlist)
                x.Status = 2;
            dl.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               View Support Tickets
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewSupportTickets()
        {
            DataLayer dl = new DataLayer();
            SupportTicketsVM stvm = new SupportTicketsVM();
            stvm.stList = (from o in dl.supportTickets
                                        where o.Resolved==0
                                        select o).ToList<SupportTicket>();

            stvm.qstList = (from o in dl.quickSupportTickets
                           where o.Resolved == 0
                           select o).ToList<QuickSupportTicket>();

            return View(stvm);
        }

        public ActionResult CloseByJSON(int id)
        {
            DataLayer dl = new DataLayer();
            List<SupportTicket> list = (from o in dl.supportTickets
                                             where o.ID == id
                                            select o).ToList<SupportTicket>();

            foreach (SupportTicket x in list)
                x.Resolved = 1;
            dl.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult CloseByJSONQ(int id)
        {
            DataLayer dl = new DataLayer();
            List<QuickSupportTicket> list = (from o in dl.quickSupportTickets
                                        where o.ID == id
                                        select o).ToList<QuickSupportTicket>();

            foreach (QuickSupportTicket x in list)
                x.Resolved = 1;
            dl.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Delete Order
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult OrderNumber() => View();

        [HttpPost]
        public ActionResult DeleteOrder(IdentifyVM ivm)
        {
            
            DataLayer dl = new DataLayer();
            List<Order> orderlist = (from o in dl.orders
                                     where o.Status!=2 && o.InvoiceNumber==ivm.InvoiceId
                                     select o).ToList<Order>();

            if (orderlist.Count==0)
            {
                ViewData["msg"] = "No Such order";
                return View("OrderNumber");
            }

            foreach (Order x in orderlist)
                x.Status = 2;
            dl.SaveChanges();

            ViewData["msgsc"] = "Order Deleted";
            return View("OrderNumber");
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}