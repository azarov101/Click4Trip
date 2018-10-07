using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class Order
    {
        [Key]
        public int InvoiceNumber { get; set; }

        public string CreditCard { get; set; }

        public string OrderDate { get; set; }

        public int Status { get; set; }

        public string Type { get; set; }

        public double TotalPayment { get; set; }

        public string CustomerEmail { get; set; }

        public string OrderDestination { get; set; }
        //public string StartDate { get; set; }
    }
}