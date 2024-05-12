using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;

namespace ThueXeMay.Controllers
{
    public class AboutController : Controller
    {
        // GET: About
        public ActionResult Index()
        {   
            var data = myObj.contacts.FirstOrDefault();
            ViewBag.about = data.about;
            return View();
        }
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();

        public ActionResult Rate()
        {
            var items = myObj.rates.Where(i => (bool)i.IsActive).ToList();
            return PartialView("Rate", items);
        }
    }
}