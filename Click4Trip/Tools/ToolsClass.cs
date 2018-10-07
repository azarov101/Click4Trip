using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.Tools
{
    public class ToolsClass
    {
        public static bool iskDateValidForReview(DateTime dt)
        {
            if (DateTime.Compare(DateTime.Now, dt) >= 0) // negative- t1 is earlier than t2.   positive- t1 is later than t2.
                return true;
            return false;
        }

        public static DateTime getDate(string date)
        {
            string day = "" + date[0] + date[1];
            string mon = "" + date[3] + date[4];
            string year = "" + date[6] + date[7] + date[8] + date[9];
            int Numday = Int32.Parse(day);
            int Nummon = Int32.Parse(mon);
            int Numyear = Int32.Parse(year);
            DateTime dt = new DateTime(Numyear, Nummon, Numday);
            return dt;
        }

        public static double getNumOfNights(DateTime StartDate, DateTime EndDate)
        {
            return (EndDate - StartDate).TotalDays;
        }

        public static bool iskDateValidForSupport(DateTime dt)
        {
            if (DateTime.Compare(DateTime.Now, dt) < 0) // negative- t1 is earlier than t2.   positive- t1 is later than t2.
                return true;
            return false;
        }

        public static string TimeDifferences(string t1, string t2)
        {
            TimeSpan ts1 = TimeSpan.Parse(t1);
            TimeSpan ts2 = TimeSpan.Parse(t2);

            string val = (ts2 - ts1).ToString().Substring(0,5);
            if ((ts2 - ts1).Ticks < 0)
            {
                TimeSpan ts3 = TimeSpan.Parse("23:59");
                TimeSpan ts4 = TimeSpan.Parse("00:01");

                return (ts4 + ts3 + ts2 - ts1).ToString().Substring(0, 5);
            }
            return val;
        }

        public static string DayOfTheWeek(string date)
        { // dd.mm.yyyy
            int day = Int32.Parse(date.Substring(0, 2));
            int month = Int32.Parse(date.Substring(3, 2));
            int year = Int32.Parse(date.Substring(6, 4));

            DateTime dateValue = new DateTime(year, month, day);
            return dateValue.DayOfWeek.ToString();
        }

        public static double getPricePercent(string composition)
        {
            switch(composition)
            {
                case "Couple":
                    return 2;
                case "Single":
                    return 1;
                case "Adult + Child":
                    return 1.6;
                case "Adult + 2 Children":
                    return 2.2;
                case "Adult + 3 Children":
                    return 2.8;
                case "Couple + Child":
                    return 2.6;
                case "Couple + 2 Children":
                    return 3.2;
                case "Couple + 3 Children":
                    return 3.8;
                case "3 Adults":
                    return 3;
            }
            return 1;
        }

        public static string fixReview(string rev)
        {
            rev = rev.ToLower();
            string outstr = "";
            foreach(char c in rev)
            {
                if (c >= 'a' && c <= 'z')
                    outstr += c;
                if (c == ' ')
                    outstr += "%20";
            }

            return outstr;
        }
    }
}