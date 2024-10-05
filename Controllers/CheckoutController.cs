using ASP.Net_EzShoper.Areas.Admin.Repository;
using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP.Net_EzShoper.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;


        public CheckoutController(DataContext context , IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;

        }
        public async Task<ActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();

                var orderItem = new OrderModel();

                orderItem.OrderCode = ordercode;

                orderItem.UserName = userEmail;

                orderItem.Status = 1;

                orderItem.CreatedDate = DateTime.Now;

                _dataContext.Add(orderItem);

                _dataContext.SaveChanges();

                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetailsModel();

                    orderdetails.UserName = userEmail;

                    orderdetails.OrderCode = ordercode;

                    orderdetails.ProductId = cart.ProductId;

                    orderdetails.Price = cart.Price;

                    orderdetails.Quantity = cart.Quantity;

                    _dataContext.Add(orderdetails);

                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");

                // Gửi Email sau khi đặt hàng 
                var receiver = "phamnhusondolla@gmail.com";

                var subject = "Đặt hàng thành công !";

                var message = "Đặt hàng thành công , trải nghiệm dịch vụ nhé !";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Taọ đơn hàng thành công! Vui lòng kiểm tra email của bạn và chờ để được duyệt đơn hàng!";

                return RedirectToAction("Index", "Cart");
            }
            return View();
        }
    }
}
