using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class PackageIndexVM
    {
        public List<string> countries { get; set; }

        public List<string> cities { get; set; }

        public string composition { get; set; }

        public List<string> compositions { get; set; } = new List<string>()
        {"Couple","Single","Adult + Child","Adult + 2 Children","Adult + 3 Children","Couple + Child","Couple + 2 Children","Couple + 3 Children","3 Adults" };

        [Required(ErrorMessage = "Country is required")]
        public string desCountry { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string desCity { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "Start date: Please enter a valid date")]
        public string sdate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [RegularExpression("^[0-9]{2}.[0-9]{2}.[0-9]{4}$", ErrorMessage = "End date: Please enter a valid date")]
        public string edate { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string desCode { get; set; }
    }
}