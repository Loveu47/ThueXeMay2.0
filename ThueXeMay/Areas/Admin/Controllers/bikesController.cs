using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;

namespace ThueXeMay.Areas.Admin.Controllers
{
    public class bikesController : BaseController
    {
        private RENT_MOTOREntities db = new RENT_MOTOREntities();

        // GET: Admin/bikes
        public ActionResult Index()
        {
            var bikes = db.bikes.Include(b => b.types);
            return View(bikes.ToList());
        }

        // GET: Admin/bikes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bike bike = db.bikes.Find(id);
            if (bike == null)
            {
                return HttpNotFound();
            }
            return View(bike);
        }

        // GET: Admin/bikes/Create
        public ActionResult Create()
        {
            ViewBag.id_type = new SelectList(db.types, "id_type", "type1");
            return View();
        }

        // POST: Admin/bikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "id_bike,name,price,IsActive,id_type,IsHot,describe,mass,volumn,size,consume,status")] bike bike, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                string _fn = Path.GetFileName(image.FileName);
                string path = Path.Combine(Server.MapPath("/Content/images/xe/"), _fn);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    image.SaveAs(path);
                }
                else
                {
                    image.SaveAs(path);
                }
                bike.image = "/Content/images/xe/" + _fn;
            }
            if (ModelState.IsValid)
            {
                db.bikes.Add(bike);
                db.SaveChanges();
                ThongBao("Thêm thành công!!!", "success");
                return RedirectToAction("Index");
            }

            ViewBag.id_type = new SelectList(db.types, "id_type", "type1", bike.id_type);
            return View(bike);
        }

        // GET: Admin/bikes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bike bike = db.bikes.Find(id);
            if (bike == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_type = new SelectList(db.types, "id_type", "type1", bike.id_type);
            return View(bike);
        }

        // POST: Admin/bikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "id_bike,name,price,IsActive,id_type,IsHot,describe,mass,volumn,size,consume,status")] bike bike, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                string _fn = Path.GetFileName(image.FileName);
                string path = Path.Combine(Server.MapPath("/Content/images/xe/"), _fn);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    image.SaveAs(path);
                }
                else
                {
                    image.SaveAs(path);
                }
                bike.image = "/Content/images/xe/" + _fn;
            } else if (image == null)
            {
                bike bikes = db.bikes.Where(i=>i.id_bike == bike.id_bike).FirstOrDefault();
                bike.image=bikes.image;
            }
            if (ModelState.IsValid)
            {
                db.Set<bike>().AddOrUpdate(bike); 
                db.SaveChanges();
                ThongBao("Sửa thành công!!!", "success");
                return RedirectToAction("Index");
            }
            ViewBag.id_type = new SelectList(db.types, "id_type", "type1", bike.id_type);
            return View(bike);
        }

        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bike bike = db.bikes.Find(id);
            if (bike == null)
            {
                return HttpNotFound();
            }
            db.bikes.Remove(bike);
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
