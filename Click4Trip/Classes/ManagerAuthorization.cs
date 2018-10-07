using Click4Trip.DAL;
using Click4Trip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Click4Trip.Classes
{
    public class ManagerAuthorization : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;

            string CurrentUser = httpContext.User.Identity.Name; // Current UserName //
            DataLayer dal = new DataLayer();

            List<User> usr =
                (from x in dal.users
                 where x.Email == CurrentUser
                 select x).ToList<User>();

            if (usr.Count == 1)
                if (usr.First().Role == 1)
                    return true;

            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult();
            filterContext.Result = new HttpStatusCodeResult(403);
        }
    }
}