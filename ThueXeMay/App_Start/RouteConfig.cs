using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ThueXeMay
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "KiemTraDonThue",
            url: "Kiemtradonthue/{value}",
            defaults: new { controller = "Home", action = "Kiemtradonthue", value = UrlParameter.Optional } ,
            new[] { "ThueXeMay.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "ThueXeMay.Controllers" }
            );
            
        }
    }
}
