using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class RoomVM
    {
        public double roomPrice { get; set; }

        public string roomDescription { get; set; }

        public List<string> roomType { get; set; }

        public string roomComposition { get; set; }

    }
}