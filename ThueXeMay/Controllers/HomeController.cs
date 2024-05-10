//Đức Tài :v
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;
namespace ThueXeMay.Controllers
{
    public class HomeController : Controller
    {
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Menu()
        {
            var item = myObj.menus.Where(i => (bool)i.IsActive).OrderBy(j => j.order).ToList();
            return PartialView("Menu", item);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Layout_Contact()
        {
            var items = myObj.contacts.Take(1);
            return PartialView("Layout_Contact", items);
        }
        public ActionResult DkDv()
        {
            return View("DK_DV");
        }
        public ActionResult faqs()
        {
            return View("FAQs");
        }
        public ActionResult Kiemtradonthue(string value)
        {
            int id;
            if (value == null || int.TryParse(value, out id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = int.Parse(value);
            rent rent = myObj.rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            var checkbill = myObj.bills.Where(i => i.id_rent == rent.id_rent).FirstOrDefault();
            if (checkbill == null)
            {
                ViewBag.trangthai = "Đã đặt đơn thành công!!!";
            }
            else
            {
                
                if (checkbill.date_end == null)
                {
                    ViewBag.trangthai = "Đơn đang thuê!!!";
                    return View(checkbill);
                }
                else
                {
                    ViewBag.trangthai = "Đã hoàn thành!!!";
                }
            }
            return View();
        }
    }
}
