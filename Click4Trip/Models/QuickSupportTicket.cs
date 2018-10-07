using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class QuickSupportTicket
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "We need to know the problem")]
        public string Note { get; set; }

        public int InvoiceId { get; set; }

        public string SupportType { get; set; }

        public int Resolved { get; set; }
    }
}