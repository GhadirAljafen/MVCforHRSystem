using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
//using IAuthenticationFilter = System.Web.Http.Filters.IAuthenticationFilter;

namespace HR.Website.Infrastructure
{
    public class CustomAuthorizationFilter : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        public CustomAuthorizationFilter(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            
            bool authorize = false;
            //if (httpContext.Session["Role"] != null)
            //{
                var roleType = Convert.ToString(httpContext.Session["Role"]);
                foreach (var role in allowedroles)
                {
                    if (role == roleType)
                        return true;
                }
            //}
                return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "User" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}