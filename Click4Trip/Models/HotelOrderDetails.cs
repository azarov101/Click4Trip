using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class HotelOrderDetails
    {
        [Key]
        public int ID { get; set; } // not realy needed - added only because the DB was showing an error

        public int Invoice { get; set; }

        /*public int NumOfAdults { get; set; }*/

        /*public int NumOfChilds { get; set; }*/

        /*public int NumOfBabies { get; set; }*/

        public string RoomType { get; set; }

        public string RoomComposition { get; set; }

        /*public string HostingType { get; set; }*/

        public double PaymentForRoom { get; set; }
    }
}