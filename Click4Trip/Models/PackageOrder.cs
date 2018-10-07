using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class PackageOrder
    {
        [Key]
        public int Id { get; set; }
        public int InvoiceID { get; set; }
        public string Composition { get; set; }

        public string DepartureLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public int Outbound_FlightNumber { get; set; }
        public int Inbound_FlightNumber { get; set; }
        public string Airline { get; set; }
        public int NumberOfTickets { get; set; }

        public string HotelName { get; set; }
        public int NumberOfRooms { get; set; }
        public string RoomDescription { get; set; }
        public int Reviewed { get; set; }
    }
}