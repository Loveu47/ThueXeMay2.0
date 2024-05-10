using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;
using System.Data.Entity;
using System.Xml.Linq;
using System.Net;

namespace ThueXeMay.Controllers
{
    public class CarsController : Controller
    {
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();
        // GET: Cars
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Index(string Name, int Type = 0)
        {
            var typees = from i in myObj.types select i;
            ViewBag.Type = new SelectList(typees, "id_type", "type1");
            var names = myObj.bikes.Include(j=>j.types);
            if (!string.IsNullOrEmpty(Name))
            {
                names = names.Where(i => i.name.Contains(Name));
            }
            if (Type != 0)
            {
                names = names.Where(j => j.id_type == Type);
            }
            return View(names.ToList());
        }
        public ActionResult CarList_Home()
        {
            var items = myObj.bikes.Include(j => j.types);
            items = items.Where(i => (bool)i.IsActive).Where(j => (bool)j.IsHot);
            return PartialView("CarsList",items.ToList());
        }
        public ActionResult Details(int? idx)
        {
            if (idx == null) return HttpNotFound();
            else
            {
                var items = myObj.bikes.Find(idx); 
                return View("Detail", items);
            }
        }
    }
}
