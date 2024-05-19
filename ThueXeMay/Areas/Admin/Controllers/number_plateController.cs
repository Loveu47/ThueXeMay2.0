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
    public class number_plateController : BaseController
    {
        private RENT_MOTOREntities db = new RENT_MOTOREntities();

        // GET: Admin/number_plate
        public ActionResult Index()
        {
            var number_plate = db.number_plate.Include(n => n.bikes);
            return View(number_plate.ToList());
        }

        // GET: Admin/number_plate/Create
        public ActionResult Create()
        {
            ViewBag.id_bike = new SelectList(db.bikes, "id_bike", "name");
            return View();
        }

        // POST: Admin/number_plate/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_bike,number,available")] number_plate number_plate)
        {
            if (ModelState.IsValid)
            {   
                var bike = db.bikes.Find(number_plate.id_bike);
                if (bike == null)
                {
                    return View("Error");
                }
                var check = db.number_plate.Where(i => i.id_bike == number_plate.id_bike).Count();
                if (check < bike.total)
                {
                    db.number_plate.Add(number_plate);
                    db.SaveChanges();
                    ThongBao("Thêm thành công!!!", "success");
                    return RedirectToAction("Index");

                } else
                {
                    ThongBao("Số lượng biển không thể quá tổng số xe hiện tại!!!","error");
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.id_bike = new SelectList(db.bikes, "id_bike", "name", number_plate.id_bike);
            return View(number_plate);
        }

        // GET: Admin/number_plate/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            number_plate number_plate = db.number_plate.Find(id);
            if (number_plate == null)
            {
                return HttpNotFound();
            }
            db.number_plate.Remove(number_plate);
            db.SaveChanges();
            ThongBao("Xoá thành công!!!", "success");
            return RedirectToAction("Index");
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
