using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Click4Trip.Controllers
{
    public class SmartAgentController : Controller
    {
        // GET: SmartAgent
        public ActionResult Index()
        {
            return View();
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //                                               Smart Agent
        public ActionResult SmartAgent()
        {
            return View();
        }

        private SmartAgentVM FixDaniesObject(SmartAgentVM obj)
        {
            switch (obj.Question1)
            {
                case "Friends":
                    obj.Friends = true;
                    break;
                case "Family":
                    obj.Family = true;
                    break;
                case "Partner":
                    obj.Partner = true;
                    break;
                case "Alone":
                    obj.Alone = true;
                    break;
            }

            switch (obj.Question3)
            {
                case "Fall":
                    obj.Fall = true;
                    break;
                case "Winter":
                    obj.Winter = true;
                    break;
                case "Spring":
                    obj.Spring = true;
                    break;
                case "Summer":
                    obj.Summer = true;
                    break;
            }
            return obj;
        }
        [HttpPost]
        public ActionResult CalculateResults(SmartAgentVM obj)
        {
            obj = FixDaniesObject(obj);

            DataLayer dl = new DataLayer();
            // get all rows from database
            List<SmartAgent> smartAgent_LST = (from u in dl.smartAgent                                             
                                             select u).ToList<SmartAgent>();

            int[] arr = Enumerable.Repeat(-1, smartAgent_LST.Count).ToArray();
            int importantField = 3; // the value of important fields will be greater

            // first question:
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                if (obj.Friends && smartAgent_LST[i].Friends != 1) { arr[i] = smartAgent_LST[i].Friends * importantField; }
                else if (obj.Family && smartAgent_LST[i].Family != 1) { arr[i] = smartAgent_LST[i].Family * importantField; }
                else if (obj.Partner && smartAgent_LST[i].Partner != 1) { arr[i] = smartAgent_LST[i].Partner * importantField; }
                else if (obj.Alone && smartAgent_LST[i].JustMe != 1) { arr[i] = smartAgent_LST[i].JustMe * importantField; }
            }

            // second question:
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                if (arr[i] == -1) { continue; }

                if (obj.Extreme && smartAgent_LST[i].Extreme != 1) { arr[i] += smartAgent_LST[i].Extreme; }
                if (obj.Romantic && smartAgent_LST[i].Romantic != 1) { arr[i] += smartAgent_LST[i].Romantic; }
                if (obj.Trek && smartAgent_LST[i].Trek != 1) { arr[i] += smartAgent_LST[i].Trek; }
                if (obj.Nightlife && smartAgent_LST[i].NightLife != 1) { arr[i] += smartAgent_LST[i].NightLife; }
                if (obj.Culture_Art && smartAgent_LST[i].CultureArt != 1) { arr[i] += smartAgent_LST[i].CultureArt; }
                if (obj.Rest && smartAgent_LST[i].JustRest != 1) { arr[i] += smartAgent_LST[i].JustRest; }
                if (obj.FamilyFun && smartAgent_LST[i].FamilyTrip != 1) { arr[i] += smartAgent_LST[i].FamilyTrip; }
            }

            // third question:
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                if (arr[i] == -1) { continue; }

                if (obj.Fall && smartAgent_LST[i].Autumn != 1) { arr[i] += smartAgent_LST[i].Autumn * importantField; }
                else if (obj.Winter && smartAgent_LST[i].Winter != 1) { arr[i] += smartAgent_LST[i].Winter * importantField; }
                else if (obj.Spring && smartAgent_LST[i].Spring != 1) { arr[i] += smartAgent_LST[i].Spring * importantField; }
                else if (obj.Summer && smartAgent_LST[i].Summer != 1) { arr[i] += smartAgent_LST[i].Summer * importantField; }

                // if this location is not suitable at all - rule out this location (set -1)
                if (obj.Fall && smartAgent_LST[i].Autumn == 1) { arr[i] = -1; }
                else if (obj.Winter && smartAgent_LST[i].Winter == 1) { arr[i] = -1; }
                else if (obj.Spring && smartAgent_LST[i].Spring == 1) { arr[i] = -1; }
                else if (obj.Summer && smartAgent_LST[i].Summer == 1) { arr[i] = -1; }
            }

            // forth question:
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                if (arr[i] == -1) { continue; }

                if (obj.Music && smartAgent_LST[i].Music != 1) { arr[i] += smartAgent_LST[i].Music; }
                if (obj.Sport && smartAgent_LST[i].Sport != 1) { arr[i] += smartAgent_LST[i].Sport; }
                if (obj.Food_Drinks && smartAgent_LST[i].FoodDrinks != 1) { arr[i] += smartAgent_LST[i].FoodDrinks; }
                if (obj.Museums && smartAgent_LST[i].Museums != 1) { arr[i] += smartAgent_LST[i].Museums; }
                if (obj.Photography && smartAgent_LST[i].Photography != 1) { arr[i] += smartAgent_LST[i].Photography; }
                if (obj.Shopping && smartAgent_LST[i].Shopping != 1) { arr[i] += smartAgent_LST[i].Shopping; }
            }

            // fifth question:
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                if (arr[i] == -1) { continue; }

                if (obj.Urban && smartAgent_LST[i].Urban != 1) { arr[i] += smartAgent_LST[i].Urban; }
                if (obj.Coastal && smartAgent_LST[i].BeachLakesRivers != 1) { arr[i] += smartAgent_LST[i].BeachLakesRivers; }
                if (obj.Tropical && smartAgent_LST[i].ForestsNature != 1) { arr[i] += smartAgent_LST[i].ForestsNature; }
                if (obj.Snowy && smartAgent_LST[i].Snowy != 1) { arr[i] += smartAgent_LST[i].Snowy; }
            }

            // sort array from high to low
            int[] indexes = new int[smartAgent_LST.Count];
            for (int i = 0; i < smartAgent_LST.Count; ++i)
            {
                indexes[i] = i;
            }
            Array.Sort(arr, indexes);

            // return top 5 highest results
            List<SmartAgentResultsVM> results = new List<SmartAgentResultsVM>();
            for (int i = smartAgent_LST.Count-1; i >= smartAgent_LST.Count - 5; --i)
            {
                if (arr[i] == -1)
                    break;

                SmartAgentResultsVM temp = new SmartAgentResultsVM()
                {
                    Location = smartAgent_LST[indexes[i]].Location,
                    Image = smartAgent_LST[indexes[i]].Image,
                    Description = smartAgent_LST[indexes[i]].Description,
                };
                results.Add(temp);
            }

            return View("SmartAgentResults", results); // go to danies view
        }
    }
}