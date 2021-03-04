using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Anz.LMJ.FrontEnd
{
    public class CheckUserSession : ActionFilterAttribute
    {
      
            public string ActionName = "Login";
            public string ControllerName = "Admin";



            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                try
                {

                    HttpSessionStateBase session = filterContext.HttpContext.Session;
                    if (session != null && session["UserId"] == null)
                    {

                        filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary {
                                { "Controller", ControllerName },
                                { "Action", ActionName }
                                  });


                    }
                }
                catch (Exception ex)
                {

                    filterContext.Result = new RedirectToRouteResult(
                  new RouteValueDictionary {
                                { "Controller", "Error" },
                                { "Action", "Index" }
                              });
                }
            }
        }
}