//Đức Tài :v
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using ThueXeMay.Areas.Admin.Data;
using ThueXeMay.Models;

namespace ThueXeMay.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();
        // GET: Admin/Home
        public class ch
        {
            public int country;
            public int value;
        }
        public ActionResult Index()
        {
            TempData["CountBikes"] = myObj.bikes.Count();
            TempData["CountBlogs"] = myObj.blogs.Count();
            TempData["CountMails"] = myObj.mails.Count();
            TempData["CountRents"] = myObj.rents.Count();
            var mo = DateTime.Now.Month;
            var item = from i in myObj.rents
                       where i.date.Value.Month == mo
                       group i.id_rent by i.date.Value.Day into h
                       select new ch()
                       {
                           country = h.Key,
                           value = h.Count(),
                       };
            return View(item);
        }
        public ActionResult Mail()
        {
            var item = myObj.mails.Where(i => i.IsRead == false).OrderByDescending(i => i.ContactId).ToList();
            ViewBag.Mail = item.Count();
            return PartialView("_Mail", item);
        }
        public ActionResult AllMail()
        {
            var item = myObj.mails.OrderByDescending(i => i.ContactId).ToList();
            return View("AllMail", item);
        }
        public ActionResult DeleteMail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mail mail = myObj.mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            myObj.mails.Remove(mail);
            myObj.SaveChanges();
            ThongBao("Xoá thành công!!!", "success");
            return RedirectToAction("AllMail");
        }

        public ActionResult MailDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mail type = myObj.mails.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            type.IsRead = true;
            myObj.SaveChanges();
            return View("Mail", type);
        }
        public ActionResult ContactAdmin()
        {
            var item = myObj.contacts.Single();
            return View("ContactAdmin", item);
        }
        public ActionResult EditContact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contact contact = myObj.contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Admin/contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContact([Bind(Include = "id,address,phone,email,facebook_link")] contact contact)
        {
            if (ModelState.IsValid)
            {
                myObj.Entry(contact).State = System.Data.Entity.EntityState.Modified;
                myObj.SaveChanges();
                return RedirectToAction("ContactAdmin");
            }
            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RepMail(FormCollection data)
        {
            var email = data["mail"];
            var body = data["content"];
            try
            {
                var message = new MailMessage()
                {
                    IsBodyHtml = true
                };
                message.To.Add(email);
                message.Subject = "Phản hồi thư";
                message.Body = body;
                var smtp = new SmtpClient();
                smtp.Send(message);
                ThongBao("Đã gửi phản hồi thành công!!", "success");
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        public async Task<ActionResult> GetBank()
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                var response = await _httpClient.GetAsync("https://api.vietqr.io/v2/banks");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    BankData result = JsonConvert.DeserializeObject<BankData>(content);
                    if (result != null)
                    {
                        var resultt = result.Getdata();
                        return View(resultt);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult EditAbout()
        {
            var about = myObj.contacts.FirstOrDefault();
            ViewBag.about = about.about;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditAbout(FormCollection form)
        {
            var about = form["about"];
            if (about == null)
            {
                ThongBao("Vui lòng nhập đủ thông tin!!!", "error");
                return View();
            }
            var data = myObj.contacts.FirstOrDefault();
            data.about = about;
            myObj.contacts.AddOrUpdate(data);
            myObj.SaveChanges();
            ThongBao("Thành công!!!", "success");
            return RedirectToAction("EditAbout");
        }

        public ActionResult Bank()
        {   
            var bank = myObj.contacts.FirstOrDefault();
            return View("Bank",bank);
        }
        public ActionResult EditBank()
        {
            contact bank = myObj.contacts.FirstOrDefault();
            return View(bank);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBank(FormCollection form)
        {
            var bin = form["bank_bin"];
            var number = form["bank_number"];
            var name = form["bank_name"];
            var bank = myObj.contacts.FirstOrDefault();
            if (bank == null || number == null || name == null)
            {
                ThongBao("Vui lòng nhập đủ thông tin!!!", "error");
                return View();
            }
            bank.bank_bin = bin;
            bank.bank_number = number;
            bank.bank_name = name;
            myObj.contacts.AddOrUpdate();
            myObj.SaveChanges();
            ThongBao("Thành công!!!", "success");
            return RedirectToAction("Bank");
        }
        public ActionResult ThongKe()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ThongKe(int? month, int? year)
        {   
            if(month == null && year == null)
            {
                ViewBag.tb = "";
                return View();
            }
            if (month != null) {
                var item = from i in myObj.bills
                           where i.date_end != null && i.date_end.Value.Month == month & i.date_end.Value.Year == year
                           group i.total by i.date_end.Value.Day into h
                           select new ch()
                           {
                               country = h.Key,
                               value = (int)h.Sum(),
                           };
                ViewBag.tb = "Doanh thu theo ngày của tháng: " +month +"/"+ year;
                return View(item);
            } else
            {
                var item = from i in myObj.bills
                           where i.date_end.Value.Year == year
                           group i.total by i.date_end.Value.Month into h
                           select new ch()
                           {
                               country = h.Key,
                               value = (int)h.Sum(),
                           };
                ViewBag.tb = "Doanh thu theo tháng của năm: " + year;
                return View(item);
            }
            
            //if (option == "thang")
            //{
            //    var total = myObj.bills.Where(i=>i.date_end.Value.Month == value).Select(j=>j.total).Sum();
            //    ViewBag.total = total;
            //    return View();
            //}
            //if (option == "ngay")
            //{
            //    var total = myObj.bills.Where(i => i.date_end.Value.Day == value).Select(j => j.total).Sum();
            //    ViewBag.total = total;
            //    return View();
            //}
            //if (option == "nam")
            //{
            //    var total = myObj.bills.Where(i => i.date_end.Value.Year == value).Select(j => j.total).Sum();
            //    ViewBag.total = total;
            //    return View();
            //}
           
        }
    }
}