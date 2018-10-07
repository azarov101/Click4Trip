using Click4Trip.Classes;
using Click4Trip.DAL;
using Click4Trip.Models;
using Click4Trip.Tools;
using Click4Trip.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Click4Trip.Controllers
{
    [ManagerAuthorization]
    public class ManagementController : Controller
    {
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult GetOrdersByJSON()
        {
            DataLayer dl = new DataLayer();
            List<Order> orderList = dl.orders.ToList<Order>();

            return Json(orderList, JsonRequestBehavior.AllowGet);
        }

        //                                               Add User
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult AddUser() => View();

        [HttpPost]
        public ActionResult SubmitUser(User user)
        {
            if (ModelState.IsValid)
            {
                DataLayer dl = new DataLayer();
                List<User> userToCheck = (from u in dl.users
                                             where u.Email.ToUpper() == user.Email.ToUpper()
                                             select u).ToList<User>();

                if (userToCheck.Count >= 1)
                {
                    ViewData["msg"] = "Username already exists!";
                    return View("AddUser", user);
                }

                Encryption encryption = new Encryption();
                string hashAndSalt = encryption.CreateHash(user.Pass);
                string[] split = hashAndSalt.Split(':');

                UserSalt us = new UserSalt() { Email= user.Email, Salt = split[0] };
                UserPass up= new UserPass() { Email = user.Email, Password = split[1] };

                dl.users.Add(user);
                dl.userPass.Add(up);
                dl.userSalt.Add(us);
                dl.SaveChanges();

                ViewData["msgsc"] = "User added!";
                return View("AddUser", new User());
            }
            return View("AddUser", user);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Delete User
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult DeleteUser()
        {
            DataLayer dl = new DataLayer();
            List<string> emails = (from u in dl.users
                                   select u.Email).ToList<string>();
            EmailsVM evm = new EmailsVM();
            evm.emails = emails;
            return View(evm);
        }

        [HttpPost]
        public ActionResult SubmitDeleteUser(EmailsVM evm)
        {
            DataLayer dl = new DataLayer();
            List<string> email = (from u in dl.users
                                  where u.Email.ToUpper() == evm.selectedEmail.ToUpper()
                                  select u.Email).ToList<string>();
            List<string> emails = (from u in dl.users
                                   select u.Email).ToList<string>();
            evm.emails = emails;

            if (email.Count == 1)
            {
                string eml = email.FirstOrDefault().ToUpper();

                User usr = (from u in dl.users
                               where u.Email.ToUpper() == eml
                               select u).ToList<User>().FirstOrDefault();


                UserSalt us = (from u in dl.userSalt
                               where u.Email.ToUpper() == eml
                               select u).ToList<UserSalt>().FirstOrDefault();

                UserPass up = (from u in dl.userPass
                               where u.Email.ToUpper() == eml
                               select u).ToList<UserPass>().FirstOrDefault();

                dl.userSalt.Remove(us);
                dl.userPass.Remove(up);
                dl.users.Remove(usr);
                dl.SaveChanges();
                ViewData["msg"] = "User deleted!";
                return View("DeleteUser", evm);
            }
            ViewData["msg"] = "User does not exist!";
            return View("DeleteUser", evm);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Restore Password
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult RestorePassword()
        {
            DataLayer dl = new DataLayer();
            EmailsVM evm = new EmailsVM();
            evm.emails = (from u in dl.users
                          select u.Email).ToList<string>();
            return View(evm);
        }

        public ActionResult GetUserByJSON(string em)
        {
            DataLayer dl = new DataLayer();
            List<User> userslist = (from x in dl.users
                                       where x.Email.ToUpper() == em.ToUpper()
                                       select x).ToList<User>();

            return Json(userslist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitCngPass(EmailsVM evm)
        {
            DataLayer dl = new DataLayer();
            User oldUser = (from x in dl.users
                               where x.Email.ToUpper() == evm.selectedEmail.ToUpper()
                               select x).ToList<User>().FirstOrDefault();

            Encryption encryption = new Encryption();
            string hashAndSalt = encryption.CreateHash(evm.password);
            string[] split = hashAndSalt.Split(':');

            UserSalt us = (from u in dl.userSalt
                           where u.Email.ToUpper() == oldUser.Email
                           select u).ToList<UserSalt>().FirstOrDefault();

            UserPass up = (from u in dl.userPass
                           where u.Email.ToUpper() == oldUser.Email
                           select u).ToList<UserPass>().FirstOrDefault();

            dl.userSalt.Remove(us);
            dl.userPass.Remove(up);

            us = new UserSalt() { Email = oldUser.Email, Salt = split[0] };
            up = new UserPass() { Email = oldUser.Email, Password = split[1] };

            dl.userSalt.Add(us);
            dl.userPass.Add(up);

            dl.SaveChanges();
            ViewData["msg"] = "User's password changed!";
            evm.emails = (from u in dl.users
                          select u.Email).ToList<string>();
            return View("RestorePassword", evm);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Smart Agent Add Cities
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult AddCities() => View();

        [HttpPost]
        public ActionResult SubmitAddCty(SmartAgent sa)
        {
            DataLayer dl = new DataLayer();
            List<SmartAgent> cities = (from x in dl.smartAgent
                            where x.Location.ToUpper() == sa.Location.ToUpper()
                            select x).ToList<SmartAgent>();
            if (cities.Count != 0)
                ViewData["msg"] = "City already exist!";
            else if (sa.Location==null)
                ViewData["msg"] = "Location required!";

            else
            {
                dl.smartAgent.Add(sa);
                dl.SaveChanges();
                ViewData["msgsc"] = "City Added!";
            }

            return View("AddCities", sa);
        }
        //-----------------------------------------------------------------------------------------------------------------------------



        //                                               Smart Agent Delete Cities
        //-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult DeleteCities()
        {
            DataLayer dl = new DataLayer();
            CitiesVM cvm = new CitiesVM();
            cvm.cities = (from u in dl.smartAgent
                                   select u.Location).ToList<string>();
            
            return View(cvm);
        }

        [HttpPost]
        public ActionResult SubmitDeleteCity(CitiesVM cvm)
        {
            DataLayer dl = new DataLayer();
            List<string> cities = (from u in dl.smartAgent
                                   where u.Location.ToUpper() == cvm.selectedCity.ToUpper()
                                  select u.Location).ToList<string>();
            cvm.cities = (from u in dl.smartAgent
                          select u.Location).ToList<string>();

            if (cities.Count == 1)
            {
                string cty = cities.FirstOrDefault().ToUpper();

                SmartAgent sa = (from u in dl.smartAgent
                            where u.Location.ToUpper() == cty
                                 select u).ToList<SmartAgent>().FirstOrDefault();

                dl.smartAgent.Remove(sa);
                dl.SaveChanges();
                ViewData["msg"] = "City deleted!";
                return View("DeleteCities", cvm);
            }
            ViewData["msg"] = "City does not exist!";
            return View("DeleteCities", cvm);
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}