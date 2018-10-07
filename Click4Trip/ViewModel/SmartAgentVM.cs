using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class SmartAgentVM
    {
        public bool Friends { get; set; }
        public bool Family { get; set; }
        public bool Partner { get; set; }
        public bool Alone { get; set; }

        public bool Extreme { get; set; }
        public bool Romantic { get; set; }
        public bool Trek { get; set; }
        public bool Nightlife { get; set; }
        public bool Culture_Art { get; set; }
        public bool Rest { get; set; }
        public bool FamilyFun { get; set; }

        public bool Fall { get; set; }
        public bool Winter { get; set; }
        public bool Spring { get; set; }
        public bool Summer { get; set; }

        public bool Music { get; set; }
        public bool Sport { get; set; }
        public bool Food_Drinks { get; set; }
        public bool Museums { get; set; }
        public bool Photography { get; set; }
        public bool Shopping { get; set; }

        public bool Urban { get; set; }
        public bool Coastal { get; set; }
        public bool Tropical { get; set; }
        public bool Snowy { get; set; }

        public string Question1 { get; set; }
        public string Question3 { get; set; }
    }
}