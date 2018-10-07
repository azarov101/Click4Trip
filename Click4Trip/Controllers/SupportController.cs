using Click4Trip.Classes;
using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.Tools;
using Click4Trip.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Click4Trip.Controllers
{
    public class SupportController : Controller
    {
        //                                               Quick Support Ticket
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult ChangeOrder() => View();

        [HttpPost]
        public ActionResult SubmitIdentify(IdentifyVM ivm)
        {
            DataLayer dl = new DataLayer();
            List<Order> orderlist = (from x in dl.orders
                                     where x.CreditCard == ivm.credit && x.InvoiceNumber == ivm.InvoiceId && x.Status == 1
                                     select x).ToList<Order>();
            if (orderlist.Count == 0)
            {
                ViewData["msg"] = "Invalid Credit Card and/or Invoice!";
                return View("ChangeOrder", ivm);
            }

            int ino = orderlist.First().InvoiceNumber;

            List<QuickSupportTicket> qst = (from x in dl.quickSupportTickets
                                     where x.InvoiceId == ino && x.Resolved == 0
                                            select x).ToList<QuickSupportTicket>();

            if (qst.Count != 0)
            {
                ViewData["msg"] = "You Already Opened a support for this order!";
                return View("ChangeOrder", ivm);
            }

            string orderStartDate;

            if (orderlist[0].Type == "Hotel")
            {
                orderStartDate = (from x in dl.hotelOrders
                                  where x.InvoiceID == ino
                                  select x.StartDate).ToList<string>()[0];
            }
            else if (orderlist[0].Type == "Flight")
            {
                orderStartDate = (from x in dl.flightOrders
                                  where x.InvoiceID == ino
                                  select x.DepartureDate).ToList<string>()[0];
            }
            else if (orderlist[0].Type == "CarRent")
            {
                orderStartDate = (from x in dl.rentCarOrder
                                  where x.InvoiceID == ino
                                  select x.PickUpDate).ToList<string>()[0];
            }
            else // (orderlist[0].Type == "Package")
            {
                orderStartDate = (from x in dl.packageOrders
                                  where x.InvoiceID == ino
                                  select x.DepartureDate).ToList<string>()[0];
            }


            if (!ToolsClass.iskDateValidForSupport(ToolsClass.getDate(orderStartDate)))
            {
                ViewData["msg"] = "You Cannot Change a past Order!";
                return View("ChangeOrder", ivm);
            }

            TempData["OrderType"] = orderlist.First().Type;
            TempData["OrderDate"] = orderlist.First().OrderDate;
            TempData["CreditCard"] = orderlist.First().CreditCard;
            TempData["ID"] = orderlist.First().InvoiceNumber;
            TempData["InvoiceNumber"] = orderlist.First().InvoiceNumber;
            return View("WriteQuickNote");
        }

        [HttpPost]
        public ActionResult SubmitQuickNote(QuickSupportTicket qst)
        {
            
            if (TempData["InvoiceNumber"]==null)
            {
                ViewData["msg"] = "You Already Opened a support for this order!";
                return View("ChangeOrder");
            }
            qst.Resolved = 0;
            qst.InvoiceId = Convert.ToInt32(TempData["InvoiceNumber"].ToString());
            DataLayer dl = new DataLayer();
            dl.quickSupportTickets.Add(qst);
            dl.SaveChanges();
            return View("Success");
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Support Ticket
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult SupportTicket() => View();

        [HttpPost]
        public ActionResult SubmitTicket(SupportTicket st)
        {
            DataLayer dl = new DataLayer();
            st.Resolved = 0;
            dl.supportTickets.Add(st);
            dl.SaveChanges();
            ViewData["msg"] = "Ticket sent!";
            return View("SupportTicket");
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        
            
            
        //                                               View Order
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewOrder() => View();

        [HttpPost]
        public ActionResult SubmitVerification(IdentifyVM ivm)
        {
            DataLayer dl = new DataLayer();
            Order order = (from x in dl.orders
                           where x.CreditCard == ivm.credit && x.InvoiceNumber == ivm.InvoiceId
                           select x).ToList<Order>().First();

            if (order == null)
            {
                ViewData["msg"] = "Invalid Id and/or Invoice!";
                return View("ViewOrder");
            }

            // IN CASE OF HOTEL
            if (order.Type == "Hotel")
                return returnHotel(order);

            // IN CASE OF FLIGHT
            if (order.Type == "Flight")
                return returnFlight(order);

            // IN CASE OF PACKAGE
            if (order.Type == "Package")
                return returnPackage(order);

            // IN CASE OF CAR RENTAL
            if (order.Type == "CarRent")
                return returnCarRent(order);

            return View();
        }

        public ActionResult returnHotel(Order order)
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
            return View("ViewHotelOrderReceipt", ovm);
        }

        public ActionResult returnFlight(Order order)
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


            return View("ViewFlightOrderReceipt", ovm);
        }

        public ActionResult returnPackage(Order order)
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
            return View("ViewPackageOrderReceipt", ovm);
        }

        public ActionResult returnCarRent(Order order)
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
            return View("ViewCarRentOrderReceipt", ovm);
        }

        [HttpGet]
        public ActionResult ViewOrderReceipt(int invoice)
        {
            EmailSender es = new EmailSender();
            DataLayer dl = new DataLayer();
            Order order = (from x in dl.orders
                           where x.InvoiceNumber == invoice
                           select x).ToList<Order>().First();

            if (order.Type == "Hotel")
            {
                //renderedHTML = Controllers.SupportController.RenderViewToString("Support", "ViewHotelOrderReceiptEM", Controllers.ServiceController.returnHotel(order));
                var renderedHTML = Controllers.SupportController.RenderViewToString(this, "ViewHotelOrderReceiptEM", Controllers.ServiceController.returnHotel(order));
                es.SendMail(order.CustomerEmail, renderedHTML);
                return returnHotel(order);
            }

            // IN CASE OF FLIGHT
            if (order.Type == "Flight")
            {
                //renderedHTML = Controllers.SupportController.RenderViewToString("Support", "ViewFlightOrderReceiptEM", Controllers.ServiceController.returnFlight(order));
                var renderedHTML = Controllers.SupportController.RenderViewToString(this, "ViewFlightOrderReceiptEM", Controllers.ServiceController.returnFlight(order));
                es.SendMail(order.CustomerEmail, renderedHTML);
                return returnFlight(order);
            }
                
            // IN CASE OF PACKAGE
            if (order.Type == "Package")
            {
                //renderedHTML = Controllers.SupportController.RenderViewToString("Support", "ViewPackageOrderReceiptEM", Controllers.ServiceController.returnPackage(order));
                var renderedHTML = Controllers.SupportController.RenderViewToString(this, "ViewPackageOrderReceiptEM", Controllers.ServiceController.returnPackage(order));
                es.SendMail(order.CustomerEmail, renderedHTML);
                return returnPackage(order);
            }

            // IN CASE OF RENTCAR
            if (order.Type == "CarRent")
            {
                //renderedHTML = Controllers.SupportController.RenderViewToString("Support", "ViewCarRentOrderReceiptEM", Controllers.ServiceController.returnCarRent(order));
                var renderedHTML = Controllers.SupportController.RenderViewToString(this, "ViewCarRentOrderReceiptEM", Controllers.ServiceController.returnCarRent(order));
                es.SendMail(order.CustomerEmail, renderedHTML);
                return returnCarRent(order);
            }

            return View();
        }

        protected override void ExecuteCore() { }
        //public static string RenderViewToString(string controllerName, string viewName, object viewData)
        //{
        //    using (var writer = new StringWriter())
        //    {
        //        var routeData = new RouteData();
        //        routeData.Values.Add("controller", controllerName);
        //        var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new SupportController());
        //        var razorViewEngine = new RazorViewEngine();
        //        var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

        //        var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
        //        razorViewResult.View.Render(viewContext, writer);
        //        return writer.ToString();
        //    }
        //}
        public static string RenderViewToString(Controller controller, string viewName, object model)
        {
            var context = controller.ControllerContext;
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------

    }
}