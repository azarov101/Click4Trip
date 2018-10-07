using Click4Trip.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Click4Trip.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; }

        public int Role { get; set; }  // 1 is Manager. 2 is Sales Agent

        public string FullName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password required")]
        [StringLength(12, MinimumLength =8,ErrorMessage = "Password is 8-12 charachters")]
        public string Pass { get; set; }

        public static int getUserRole()
        {
            string mail = HttpContext.Current.User.Identity.Name;
            DataLayer dl = new DataLayer();

            User userLoggedIn = (from u in dl.users
                                 where u.Email.ToUpper() == mail.ToUpper()
                                 select u).ToList<User>().First();

            return userLoggedIn.Role;
        }

        public static string getUserName()
        {
            string mail = HttpContext.Current.User.Identity.Name;
            DataLayer dl = new DataLayer();

            User userLoggedIn = (from u in dl.users
                                 where u.Email.ToUpper() == mail.ToUpper()
                                 select u).ToList<User>().First();

            return userLoggedIn.FullName;
        }
    }
}