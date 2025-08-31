using StackOverflowService.Helpers;
using StackOverflowService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StackOverflowService.Filters
{
    public class SessionTimeoutFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            // Preskočiti proveru za Login, Logout i Register akcije
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            if (controller == "User" && (action == "Login" || action == "Logout" || action == "Register"))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Proverite istek sesije
            var currentUser = SessionHelper.GetObjectFromJson<UserSession>(httpContext.Session ,"CurrentUser");
            if (currentUser != null && currentUser.LoginTime.HasValue &&
                (DateTime.UtcNow - currentUser.LoginTime.Value).TotalMinutes > 30)
            {
                // Sesija je starija od 30 minuta, resetujte je
                httpContext.Session.Remove("CurrentUser");
                FormsAuthentication.SignOut();

                // Preusmeriti na Login
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "User" },
                        { "action", "Login" }
                    }
                );
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}