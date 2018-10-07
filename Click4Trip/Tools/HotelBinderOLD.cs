//using Click4Trip.Models;
//using Click4Trip.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Click4Trip.Tools
//{
//    public class HotelBinderOLD: IModelBinder
//    {
//        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
//        {
//            HttpContextBase requestedContext = controllerContext.HttpContext; // creacte new veraible of the requested context

//            // ---------------------------------------------------------------------
//            // find the location of the links (in the array) and the count of images
//            int[] imageArr = new int[] { 0, 0, 0, 0, 0, 0 };
//            int imageCount = 0;

//            for (int i = 0; i < 6; ++i) 
//            {
//                string temp = requestedContext.Request.Form["image[" + i + "].Link"];
//                if (temp != "")
//                {
//                    imageArr[i] = 1;
//                    ++imageCount;
//                }
//            }

//            // ---------------------------------------------------------------------
//            // find the location of the rooms (in the array) and the count of rooms
//            int roomCount = 0;
//            int[] roomArr = new int[] { 0, 0, 0, 0, 0, 0 };

//            for (int i = 0; i < 6; ++i)
//            {
//                string temp = requestedContext.Request.Form["roomPrice[" + i + "].RoomType"];
//                if (temp != "")
//                {
//                    roomArr[i] = 1;
//                    ++roomCount;
//                }
//            }

//            // ---------------------------------------------------------------------
//            // create the new HotelVM object
//            HotelVMOLD obj = new HotelVMOLD()
//            {
//                hotel = new HotelOLD()
//                {
//                    Id = 1,
//                    Name = requestedContext.Request.Form["hotel.Name"],
//                    Country = requestedContext.Request.Form["hotel.Country"],
//                    City = requestedContext.Request.Form["hotel.City"],
//                    Facilities = requestedContext.Request.Form["hotel.Facilities"],
//                    Description = requestedContext.Request.Form["hotel.Description"],
//                    Star = Int32.Parse(requestedContext.Request.Form["hotel.Star"]),
//                    AdultPrice = Int32.Parse(requestedContext.Request.Form["hotel.AdultPrice"]),
//                    ChildPrice = Int32.Parse(requestedContext.Request.Form["hotel.ChildPrice"]),
//                    BabyPrice = Int32.Parse(requestedContext.Request.Form["hotel.BabyPrice"]),                    
//                    Active = 1
//                },
//                image = new List<ImageOLD>(imageCount),
//                roomPrice = new List<RoomPriceOLD>(roomCount)
//            };
//            try
//            { // had to add try,catch blocks because the default value (box checked) of the check box making problems
//                obj.hotel.Active = Convert.ToInt32(Convert.ToBoolean(requestedContext.Request.Form["hotel.BoolValue"]));
//            } catch{} // do nothing

//            // ---------------------------------------------------------------------
//            // Add the images to the new object
//            for (int i = 0, j = 0; i < imageCount; ++i) 
//            {
//                while (imageArr[j] == 0) { ++j; }

//                ImageOLD tempImage = new ImageOLD()
//                {
//                    Id = 1, // doesn't matter
//                    Link = requestedContext.Request.Form["image[" + j + "].Link"],
//                    ObjectId = 1, // doesnt matter - need to be the id of the hotel
//                    ObjectType = "hotel"
//                };

//                obj.image.Add(tempImage);
//                ++j;
//            }

//            // ---------------------------------------------------------------------
//            // Add the rooms to the new object
//            for (int i = 0, j = 0; i < roomCount; ++i)
//            {
//                while (roomArr[j] == 0) { ++j; }

//                RoomPriceOLD tempRoom = new RoomPriceOLD()
//                {
//                    HotelId = 1, // doesnt matter - need to be the id of the hotel
//                    Price = Int32.Parse(requestedContext.Request.Form["roomPrice[" + j + "].Price"]),
//                    RoomType = requestedContext.Request.Form["roomPrice[" + j + "].RoomType"]
//                };

//                obj.roomPrice.Add(tempRoom);
//                ++j;
//            }

//            return obj;
//        }
//    }
//}