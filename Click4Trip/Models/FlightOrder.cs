using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class FlightOrder
    {
        [Key]
        public int Id { get; set; }

        public int InvoiceID { get; set; }

        public string DepartureLocation { get; set; }

        public string ArrivalLocation { get; set; }

        public string DepartureDate { get; set; }

        public string ReturnDate { get; set; }

        public int Outbound_FlightNumber { get; set; }

        public int Inbound_FlightNumber { get; set; }
        public string Airline { get; set; }

        public int NumberOfBabies { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfYoungsters { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfPensioners { get; set; }

        public double PricePerBaby { get; set; }
        public double PricePerChild { get; set; }
        public double PricePerYoungster { get; set; }
        public double PricePerAdult { get; set; }
        public double PricePerPensioner { get; set; }
    }
}