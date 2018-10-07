using Click4Trip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class SupportTicketsVM
    {
        public List<SupportTicket> stList { get; set; }

        public List<QuickSupportTicket> qstList { get; set; }
    }
}