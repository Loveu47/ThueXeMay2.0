using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;
namespace ThueXeMay.Controllers
{
    public class PricingController : Controller
    {
        // GET: Pricing
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();

        public ActionResult Index()
        {
            var items = myObj.types.ToList();
            foreach (var item in items)
            {
                item.price_day = item.price_hour * item.price_day;
                item.price_month = item.price_day * item.price_month;
            }
            return View(items);
        }
    }

}