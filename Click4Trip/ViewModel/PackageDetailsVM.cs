using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.ViewModel
{
    public class PackageDetailsVM
    {
        public List<PackageDetails> packages { get; set; } = new List<PackageDetails>();

        public double nights { get; set; }

        public int selectedPack { get; set; }

        public string composition { get; set; }

        // for google map
        public string apiKey { get; set; } = "AIzaSyCArIw-PoGMwSXFZr08nZT64BPDvix8w98";
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}