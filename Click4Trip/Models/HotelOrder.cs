using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class HotelOrder
    {
        [Key]
        public int Id { get; set; }

        public int InvoiceID { get; set; }

        public int NumOfRooms { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string HotelName { get; set; }

        public int Reviewed { get; set; }
    }
}