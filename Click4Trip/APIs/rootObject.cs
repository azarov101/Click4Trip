using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Click4Trip.AmadeusAPI
{
    public class rootObject
    {
        /*
        [JsonProperty("location")]
        public Location location { get; set; }
        public Address address { get; set; }
        public TotalPrice totalPrice { get; set; }
        public MinDailyRate minDailyRate { get; set; }
        public Contact contact { get; set; }
        public Amenity amenity { get; set; }
        public TotalAmount totalAmount { get; set; }
        public Rate rate { get; set; }
        public RoomTypeInfo roomTypeInfo { get; set; }
        public Room room { get; set; }
        public MoreRoomsAtThisHotel moreRoomsAtThisHotel { get; set; }
        public Links links { get; set; }
        public Result result { get; set; }
        public RootObject roomObject { get; set; }
        */

        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public override string ToString() => "Location = latitude - " + latitude + " longitude - " + longitude;
        }

        public class Address
        {
            public string line1 { get; set; }
            public string city { get; set; }
            public string postal_code { get; set; }
            public string country { get; set; }
            public override string ToString() => "Address = line1 - " + line1 + " city - " + city + " postal code - " + postal_code + " country - " + country;
        }

        public class TotalPrice
        {
            public string amount { get; set; }
            public string currency { get; set; }
            public override string ToString() => "TotalPrice = amount - " + amount + " currency - " + currency;
        }

        public class MinDailyRate
        {
            public string amount { get; set; }
            public string currency { get; set; }
            public override string ToString() => "MinDailyRate = amount - " + amount + " currency - " + currency;
        }

        public class Contact
        {
            public string type { get; set; }
            public string detail { get; set; }
            public override string ToString() => "Contact = type - " + type + " detail - " + detail;
        }

        public class Amenity
        {
            public string amenity { get; set; }
            public int ota_code { get; set; }
            public string description { get; set; }
            public override string ToString() => "Amenity = amenity - " + amenity + " ota_code - " + ota_code + " description - " + description;
        }

        public class TotalAmount
        {
            public string amount { get; set; }
            public string currency { get; set; }
            public override string ToString() => "TotalAmount = amount - " + amount + " currency - " + currency;
        }

        public class Rate
        {
            public string start_date { get; set; }
            public string end_date { get; set; }
            public string currency_code { get; set; }
            public double price { get; set; }
            public override string ToString() => "Rate = start_date - " + start_date + " end_date - " + end_date + " currency_code - " + currency_code + " price - " + price;
        }

        public class RoomTypeInfo
        {
            public string bed_type { get; set; }
            public string room_type { get; set; }
            public string number_of_beds { get; set; }
            public override string ToString() => "RoomTypeInfo = bed_type - " + bed_type + " room_type - " + room_type + " number_of_beds - " + number_of_beds;

        }

        public class Room
        {
            public string booking_code { get; set; }
            public string room_type_code { get; set; }
            public string rate_plan_code { get; set; }
            public TotalAmount total_amount { get; set; }
            public List<Rate> rates { get; set; }
            public List<string> descriptions { get; set; }
            public RoomTypeInfo room_type_info { get; set; }
            public string rate_type_code { get; set; }

            public override string ToString()
            {
                string all_rates = "";
                string all_desc = "";
                foreach (Rate r in rates)
                {
                    all_rates += r.ToString() + " ";
                }
                foreach (string s in descriptions)
                {
                    all_desc += s.ToString() + " ";
                }
                return "Room = booking_code - " + booking_code + " room_type_code - " + room_type_code + " rate_plan_code - " + rate_plan_code +
                    " total_amount - " + total_amount.ToString() + " rates - " + all_rates + " descriptions - " + all_desc + " room_type_info" + room_type_info.ToString() + " rate_type_code - " + rate_type_code;

            }
        }

        public class MoreRoomsAtThisHotel
        {
            public string href { get; set; }
            public override string ToString() => "MoreRoomsAtThisHotel = href - " + href;
        }

        public class Links
        {
            public MoreRoomsAtThisHotel more_rooms_at_this_hotel { get; set; }
            public override string ToString()
            {
                return "Links = more_rooms_at_this_hotel - " + more_rooms_at_this_hotel;
            }
        }

        public class Result
        {
            public string property_code { get; set; }
            public string property_name { get; set; }
            public string marketing_text { get; set; }
            public Location location { get; set; }
            public Address address { get; set; }
            public TotalPrice total_price { get; set; }
            public MinDailyRate min_daily_rate { get; set; }
            public List<Contact> contacts { get; set; }
            public List<Amenity> amenities { get; set; }
            public List<object> awards { get; set; }
            public List<object> images { get; set; }
            public List<Room> rooms { get; set; }
            public Links _links { get; set; }

            public override string ToString()
            {
                string all_amen = "";
                string all_cont = "";
                string all_awards = "";
                string all_rooms = "";

                foreach (Amenity a in amenities)
                {
                    all_amen += a.ToString() + " ";
                }
                foreach (Contact c in contacts)
                {
                    all_cont += c.ToString() + " ";
                }
                foreach (Object o in awards)
                {
                    all_awards += o.ToString() + " ";
                }
                foreach (Room r in rooms)
                {
                    all_rooms += r.ToString() + " ";
                }
                return "Result = \nproperty_code - " + property_code + " \nproperty_name - " + property_name + " \nmarketing_text - " + marketing_text
                    + "\n" + location.ToString() + " \n" + address + " \n" + total_price
                    + " \n" + min_daily_rate + " \nContacts - " + all_cont + " \nAmenities - " + all_amen
                    + " \nawards - " + all_awards + " \nrooms - " + all_rooms + " \nlinks - " + _links.ToString();
            }
        }

        public class RootObject
        {
            public List<Result> results { get; set; }
            public override string ToString()
            {
                string res = "";
                foreach (Result r in results)
                {
                    res += r + " \n****************************************************************\n";
                }
                return "Final Result = " + res;
            }
        }
    }
}