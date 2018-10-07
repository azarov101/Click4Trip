using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class SmartAgent
    {
        [Key]
        public string Location { get; set; }

        public int Friends { get; set; }

        public int Family { get; set; }

        public int Partner { get; set; }

        public int JustMe { get; set; }

        public int Extreme { get; set; }

        public int Romantic { get; set; }

        public int Trek { get; set; }

        public int NightLife { get; set; }

        public int CultureArt { get; set; }

        public int JustRest { get; set; }

        public int FamilyTrip { get; set; }

        public int Autumn { get; set; }

        public int Winter { get; set; }

        public int Spring { get; set; }

        public int Summer { get; set; }

        public int Music { get; set; }

        public int Sport { get; set; }

        public int FoodDrinks { get; set; }

        public int Museums { get; set; }

        public int Photography { get; set; }

        public int Shopping { get; set; }

        public int Urban { get; set; }

        public int BeachLakesRivers { get; set; }

        public int ForestsNature { get; set; }

        public int Snowy { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }
    }
}