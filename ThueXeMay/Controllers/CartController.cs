//Đức Tài :v
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeMay.Models;
using System.Web.Helpers;
using System.Runtime.Serialization;
using System.Web.WebPages;

namespace ThueXeMay.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        RENT_MOTOREntities myObj = new RENT_MOTOREntities();

        public ActionResult Index()
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            return View(giohang);
        }

        public ActionResult AddCart(int id)
        {
            if (Session["giohang"] == null)
            {
                Session["giohang"] = new List<CartItem>();
            }

            List<CartItem> giohang = Session["giohang"] as List<CartItem>;

            if (giohang.FirstOrDefault(m => m.id_xe == id) == null) // ko co sp nay trong gio hang
            {
                bike sp = myObj.bikes.Find(id);

                CartItem newItem = new CartItem()
                {
                    id_xe = id,
                    name = sp.name,
                    SoLuong = 1,
                    image = sp.image,
                    price = (int)sp.price,

                };  // Tạo ra 1 CartItem mới

                giohang.Add(newItem);  // Thêm CartItem vào giỏ 
            }
            else
            {
                // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                CartItem cardItem = giohang.FirstOrDefault(m => m.id_xe == id);
                var check = myObj.bikes.Find(id);
                if (check.quantity > cardItem.SoLuong)
                {
                    cardItem.SoLuong++;
                }
            }
            return Redirect(url: Request.UrlReferrer.ToString());
        }
        public ActionResult SuaSoLuong(int SanPhamID, int soluongmoi)
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem itemSua = giohang.FirstOrDefault(m => m.id_xe == SanPhamID);
            if (itemSua != null)
            {
                itemSua.SoLuong = soluongmoi;
            }
            var check = myObj.bikes.Find(SanPhamID);
            if (check.quantity < soluongmoi)
            {   
                itemSua.SoLuong = (int)check.quantity;
                return View("QuaSoLuong");
            }
            return RedirectToAction("Index");

        }
        public RedirectToRouteResult XoaKhoiGio(int SanPhamID)
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem itemXoa = giohang.FirstOrDefault(m => m.id_xe == SanPhamID);
            if (itemXoa != null)
            {
                giohang.Remove(itemXoa);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Checkout(FormCollection frm)
        {
            List<CartItem> carts = (List<CartItem>)Session["giohang"];
            rent rent = new rent()
            {
                name = frm["inputUsername"],
                phone = frm["inputPhone"],
                mail = frm["inputEmail"],
                note = frm["inputNote"],
                date = DateTime.Now,
                date_start = frm["datestart"].AsDateTime(),
                date_end = frm["dateend"].AsDateTime()
            };
            myObj.rents.Add(rent);
            myObj.SaveChanges();
            string chitietxe = "";
            int tongtien = 0;
            foreach (CartItem item in carts)
            {   
                chitietxe = chitietxe + item.name + "*" + item.SoLuong +"<br>";
                tongtien = tongtien + item.TongTien;
                rentDetail rentDetail = new rentDetail()
                {
                    id_rent = rent.id_rent,
                    id_bike = item.id_xe,
                    amount = item.SoLuong,
                };
                var update = myObj.bikes.Find(item.id_xe);
                update.quantity = update.quantity - item.SoLuong;
                myObj.rentDetails.Add(rentDetail);
                myObj.SaveChanges();
            }

            try
            {
                var id = myObj.rents.OrderByDescending(i=>i.id_rent).FirstOrDefault();
                var message = new MailMessage()
                {
                    IsBodyHtml = true
                };
                message.To.Add(rent.mail);
                message.Subject = "Thông tin đơn thuê";
                message.Body = @"
<!doctype html>
<html lang=""vi"">
<style>
    a:hover {text-decoration: underline !important;}
</style>

<body marginheight=""0"" topmargin=""0"" marginwidth=""0"" style=""margin: 0px; background-color: #f2f3f8;"" leftmargin=""0"">
    <table cellspacing=""0"" border=""0"" cellpadding=""0"" width=""100%"" bgcolor=""#f2f3f8""
        style=""@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;"">
        <tr>
            <td>
                <table style=""background-color: #f2f3f8; max-width:670px; margin:0 auto;"" width=""100%"" border=""0""
                    align=""center"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                        <td style=""height:80px;"">&nbsp;</td>
                    </tr>
                    <!-- Logo -->
                    <tr>
                        <td style=""text-align:center;"">
                         
                            <img width=""150"" src=""https://vietup.net/files/900c89a8474476b5d9ed5738f35ab080/239221bec2543757132d37b66dfee507/logo.png"" title=""logo"" alt=""logo"">
                    
                        </td>
                    </tr>
                    <tr>
                        <td style=""height:20px;"">&nbsp;</td>
                    </tr>
                    <!-- Email Content -->
                    <tr>
                        <td>
                            <table width=""95%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0""
                                style=""max-width:670px; background:#fff; border-radius:3px;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);padding:0 40px;"">
                                <tr>
                                    <td style=""height:40px;"">&nbsp;</td>
                                </tr>
                                <!-- Title -->
                                <tr>
                                    <td style=""padding:0 15px; text-align:center;"">
                                        <h1 style=""color:#1e1e2d; font-weight:400; margin:0;font-size:32px;font-family:'Rubik',sans-serif;"">Thông tin đơn thuê</h1>
                                        <h3 style=""color:#1e1e2d; font-weight:400; margin:0;font-family:'Rubik',sans-serif;"">Bạn có thể kiểm tra trạng thái đơn tại <a href=""https://localhost:44353/Kiemtradonthue?value="+id.id_rent+@""">ĐÂY</a></h3>
                                        <span style=""display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; 
                                        width:100px;""></span>
                                    </td>
                                </tr>
                                <!-- Details Table -->
                                <tr>
                                    <td>
                                        <table cellpadding=""0"" cellspacing=""0""
                                            style=""width: 100%;font-size: 16px; border: 1px solid #ededed"">
                                            <tbody>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Mã đơn thuê:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056;"">
                                                        " + id.id_rent +  @"</td> 
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Họ và tên:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056;"">
                                                        "+ rent.name+ @"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Số điện thoại:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056;"">
                                                        "+rent.phone+@"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed;border-right: 1px solid #ededed; width: 35%; font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Email:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056;"">
                                                        "+rent.mail+@"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px;  border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%;font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Ngày giờ đặt:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056;"">
                                                        "+rent.date+@"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%;font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Thông tin xe:</td>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; color: #455056; "">
                                                        "+chitietxe+ @"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-bottom: 1px solid #ededed; width: 35%;font-weight:500; color:rgba(0,0,0,.64)"""">
                                                        Giá tiền/giờ:</td>
                                                    <td style=""padding: 10px;  border-bottom: 1px solid #ededed; color: #455056;""> " + tongtien+@"</td>
                                                </tr>
                                                <tr>
                                                    <td
                                                        style=""padding: 10px; border-right: 1px solid #ededed; width: 35%;font-weight:500; color:rgba(0,0,0,.64)"">
                                                        Ghi chú:</td>
                                                    <td style=""padding: 10px; color: #455056;""> " + rent.note+@"</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""height:40px;"">&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style=""height:20px;"">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style=""text-align:center;"">
                                <p style=""font-size:14px; color:#455056bd; line-height:18px; margin:0 0 0;""> <strong>Thuê Xe Máy - Vinh</strong></p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>

</html>";

                var smtp = new SmtpClient();
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            Session.Remove("giohang");
            return View("RentSuccess");
            //} catch
            //{
            //    return View("Error");
            //}
        }
    }
}