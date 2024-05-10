using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;
namespace ThueXeMay.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();
        public ActionResult Contact_Info()
        {
            var items = myObj.contacts.Take(1);
            return PartialView("Contact", items);
        }

        public ActionResult Create(string name, string phone, string email, string message)
        {
            try
            {
                mail myItem = new mail();
                myItem.Name = name;
                myItem.Phone = phone;
                myItem.Email = email;
                myItem.Message = message;
                myItem.CreatedDate = DateTime.Now;
                myObj.mails.Add(myItem);
                myObj.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }
        }
    }
}
