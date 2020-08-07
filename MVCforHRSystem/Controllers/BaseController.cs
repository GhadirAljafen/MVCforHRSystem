using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HR.Website.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)  
       {
            bool checkForAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (checkForAuthorization)
            {
                base.OnAuthentication(filterContext);
                return;
            }
            if (Session.Keys.Count == 0 && Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/User/Index");
                return;

            }
            base.OnAuthentication(filterContext);
        }
    }
}