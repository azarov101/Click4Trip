using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class IdentifyVM
    {
        [Required(ErrorMessage = "Invoice is required")]
        [RegularExpression("^[0-9]{1,20}$", ErrorMessage = "Invoice Is Only Numbers")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Credit Card is required")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "It Is Only 4 digits")]
        public string credit { get; set; }
    }
}