using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class HotelReview
    {
        [Key]
        public int ID { get; set; }

        public string HotelName { get; set; }

        public string Review { get; set; }

        public int ReviewType { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string ReviewDate { get; set; }
    }
}