using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
            if(rent.bills.Count() > 0)
            {
                string bienso = "Thông tin biển số: ";
                var number = db.number_plate.Where(i=>i.id_rent == id).Include(j=>j.bikes).ToList();
                foreach(var i in number)
                {   
                    var xe = db.bikes.Find(i.id_bike);
                    bienso = bienso + xe.name + ": " + i.number + ";";
                }
                ViewBag.numberplate = bienso.ToString();
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
                var update = db.bikes.Find(item.id_bike);
                update.quantity = update.quantity + item.amount;
                db.bikes.AddOrUpdate(update);
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
                rent rent = db.rents.Find(id);
                var list = rent.rentDetails.ToList();
                foreach(var item in list)
                {
                    var number = db.number_plate.Where(i=>i.id_bike == item.id_bike)
                        .Where(j=>j.available == true).OrderBy(k=>Guid.NewGuid()).Take((int)item.amount).ToList();
                    foreach(var i in number)
                    {   
                        i.available = false;
                        i.id_rent = id;
                        db.SaveChanges();
                    }
                }
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

                var number = db.number_plate.Where(j => j.id_rent == id).ToList();
                foreach (var i in number)
                {
                    i.available = true;
                    i.id_rent = null;
                    db.SaveChanges();
                }

                var list = db.rentDetails.Where(i=>i.id_rent == bill.id_rent).ToList();
                foreach(var item in list)
                {
                    var update = db.bikes.Find(item.id_bike);
                    update.quantity = update.quantity + item.amount;
                    db.bikes.AddOrUpdate();
                }
                var time = (TimeSpan)(bill.date_end - bill.date_start);
                int day = time.Days;
                int month = day / 30;
                int dayy = day % 30;
                int hour = time.Hours;
                var tigia = db.types.First();
                ViewBag.priceday = tigia.price_day;
                ViewBag.pricemonth = tigia.price_month;
                int tong = hour * bill.money_hour + (dayy) * ViewBag.priceday * bill.money_hour
       + month * ViewBag.priceday * ViewBag.pricemonth * bill.money_hour;
                bill.total = tong;
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
        
        var idx = db.bills.Where(i => i.id_rent == id).Select(j => j.id_bill).First();
        bill bill = db.bills.Find(idx);
        contact bank = db.contacts.FirstOrDefault();
        ViewBag.bin = bank.bank_bin.Replace(" ","");
        ViewBag.number = bank.bank_number.Replace(" ","");
        ViewBag.name = bank.bank_name;
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
