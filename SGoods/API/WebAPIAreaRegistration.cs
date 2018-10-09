using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace StuWeb.API
{
    public class WebAPIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SGoodsAPI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
                name: "WebApi_default",
                routeTemplate: "API/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}