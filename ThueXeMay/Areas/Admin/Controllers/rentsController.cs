using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;

namespace ThueXeMay.Areas.Admin.Controllers
{
    public class rentsController : BaseController
    {
        private RENT_MOTOREntities db = new RENT_MOTOREntities();

        // GET: Admin/rents
        public ActionResult Index()
        {
            var item = db.rents.Include(i => i.bills).OrderByDescending(j => j.id_rent).ToList();
            return View(item);
        }
        // GET: Admin/rents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rent rent = db.rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            return View(rent);
        }
        public ActionResult rentsDetail(int id)
        {
            var bike = db.rentDetails.Include(j => j.bike);
            bike = bike.Where(i => i.id_rent == id);
            return PartialView("rentsDetail", bike.ToList());
        }
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rent rent = db.rents.Find(id);
            if (rent == null)
            {
                return HttpNotFound();
            }
            List<rentDetail> rentDetail = (List<rentDetail>)db.rentDetails.Where(j => j.id_rent == id).ToList();
            foreach (var item in rentDetail)
            {
                db.rentDetails.Remove(item);
            }
            db.rents.Remove(rent);
            ThongBao("Xoá thành công!!", "success");
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult rent_start(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var check = db.bills.Where(i => i.id_rent == id).ToList();
            if (check.Count() != 0)
            {
                return Json(new { status = false });
            }
            else
            {
                bill bill = new bill()
                {
                    id_rent = (int)id,
                    date_start = DateTime.Now,
                    money_hour = (int)TempData["tong"],
                    status = "Đang thuê"
                };
                db.bills.Add(bill);
                db.SaveChanges();
                return Json(new { status = true });
            }
        }

        public ActionResult rent_end(int? id)
        {
            var idx = db.bills.Where(i => i.id_rent == id).Select(j => j.id_bill).First();
            bill bill = db.bills.Find(idx);
            if (bill == null)
            {
                return HttpNotFound();
            }
            if (bill.date_end == null)
            {
                bill.date_end = DateTime.Now;
                bill.status = "Hoàn thành";
                db.SaveChanges();
                ThongBao("Hoàn thành đơn thành công!!!", "success");
                return Redirect(url: Request.UrlReferrer.ToString());
            }
            else
            {
                ThongBao("Đơn đã hoàn thành!!", "error");
                return Redirect(url: Request.UrlReferrer.ToString());
            }
        }

    
    public ActionResult ViewBill(int? id)
    {
        var tigia = db.types.First();
        ViewBag.priceday = tigia.price_day;
        ViewBag.pricemonth = tigia.price_month;
        var idx = db.bills.Where(i => i.id_rent == id).Select(j => j.id_bill).First();
        bill bill = db.bills.Find(idx);
        return PartialView("Bill", bill);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
