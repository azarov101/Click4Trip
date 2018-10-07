using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class RentCarOrder
    {
        [Key]
        public int Id { get; set; }
        public int InvoiceID { get; set; }
        public string PickUpDate { get; set; }
        public string DropOffDate { get; set; }
        public int Days { get; set; }
        public string Address { get; set; }
        public string Provider { get; set; }
        public double Price { get; set; }
        public string AcrissCode { get; set; }
    }
}