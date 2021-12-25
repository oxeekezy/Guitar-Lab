using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GuitLab
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional }, new[] { "GuitLab.Controllers" });
            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional }, new[] { "GuitLab.Controllers" });

            routes.MapRoute("Pages","{page}",new {controller = "Pages",action = "Index"},new[]{ "GuitLab.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, new[] { "GuitLab.Controllers" });

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, new[] { "GuitLab.Controllers" });
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
           
            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional }, new[] { "GuitLab.Controllers" });
            
        }
    }
}
