//Đức Tài :v
using Microsoft.Ajax.Utilities;
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
    public class typesController : BaseController
    {
        private RENT_MOTOREntities db = new RENT_MOTOREntities();

        // GET: Admin/types
        public ActionResult Index()
        {
            return View(db.types.ToList());
        }

        // GET: Admin/types/Details/5

        // GET: Admin/types/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_type,type1,price_hour,price_day,price_month")] type type, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                string _fn = Path.GetFileName(image.FileName);
                string path = Path.Combine(Server.MapPath("/Content/images/"), _fn);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    image.SaveAs(path);
                }
                else
                {
                    image.SaveAs(path);
                }
                type.image = "/Content/images/" + _fn;
            }
            if (ModelState.IsValid)
            {
                var tigia = db.types.FirstOrDefault();
                type.price_day = type.price_day / type.price_hour;
                type.price_month = (type.price_month / type.price_hour) / type.price_day;
                db.types.Add(type);
                db.SaveChanges();
                ThongBao("Thêm thành công!!!", "success");
                return RedirectToAction("Index");
            }

            return View(type);
        }

        // GET: Admin/types/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = db.types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }
        public ActionResult Tigia()
        {
            var tigia = db.types.First();
            return View(tigia);
        }
        [HttpPost]
        public ActionResult Tigia(int day, int month)
        {
            if (ModelState.IsValid)
            {
                var types = db.types.ToList();
                foreach (var type in types)
                {
                    type.price_day = day;
                    type.price_month = month;
                }
                db.SaveChanges();
                ThongBao("Sửa thành công!!!", "success");
                return RedirectToAction("Tigia");
            }
            ThongBao("Sửa thất bại!!!", "error");
            return View();
        }
        // POST: Admin/types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_type,type1,price_hour,price_day,price_month")] type type, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                string _fn = Path.GetFileName(image.FileName);
                string path = Path.Combine(Server.MapPath("/Content/images/"), _fn);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    image.SaveAs(path);
                }
                else
                {
                    image.SaveAs(path);
                }
                type.image = "/Content/images/" + _fn;
            }
            else if (image == null)
            {
                type bikes = db.types.Where(i => i.id_type == type.id_type).FirstOrDefault();
                type.image = bikes.image;
            }
            if (ModelState.IsValid)
            {
                var tigia = db.types.First();
                type.price_day = tigia.price_day;
                type.price_month = tigia.price_month;
                db.Set<type>().AddOrUpdate(type); 
                db.SaveChanges();
                ThongBao("Sửa thành công!!!", "success");
                return RedirectToAction("Index");
            }
            ThongBao("Sửa thất bại!!!", "error");
            return View(type);
        }

        // POST: Admin/types/Delete/5

        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = db.types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            db.types.Remove(type);
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
