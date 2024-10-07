using ASP.Net_EzShoper.Areas.Admin.Repository;
using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace ASP.Net_EzShoper.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;


        public CheckoutController(DataContext context, IEmailSender emailSender)
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
                var orderItem = new OrderModel
                {
                    OrderCode = ordercode,
                    UserName = userEmail,
                    Status = 1,
                    CreatedDate = DateTime.Now
                };

                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();

                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("<h3>Cảm ơn bạn đã đặt hàng! Dưới đây là chi tiết đơn hàng của bạn:</h3>");
                messageBuilder.AppendLine("<table border='1' cellpadding='10' cellspacing='0' style='border-collapse:collapse; text-align:center;'>");
                messageBuilder.AppendLine("<tr>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Ảnh sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Danh mục sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Mã sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Thương hiệu sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Tên sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Mô tả sản phẩm</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Số lượng</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Giá</th>");
                messageBuilder.AppendLine("<th style=\"text-align:center;\">Tổng tiền</th>");
                messageBuilder.AppendLine("</tr>");

                foreach (var cart in cartItems)
                {
                   
                    var product = _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefault(p => p.Id == cart.ProductId);

                    
                    if (product != null)
                    {
                        var orderdetails = new OrderDetailsModel
                        {
                            UserName = userEmail,
                            OrderCode = ordercode,
                            ProductId = cart.ProductId,
                            Price = cart.Price,
                            Quantity = cart.Quantity
                        };

                        _dataContext.Add(orderdetails);
                        _dataContext.SaveChanges();

                        // Thêm từng dòng sản phẩm vào bảng
                        messageBuilder.AppendLine("<tr>");
                        messageBuilder.AppendLine($"<td><img src='{product.Image}' alt='{product.Name}' style='width:100px; height:auto;'/></td>");

                        // Kiểm tra Category có null không
                        var categoryName = product.Category != null ? product.Category.Name : "N/A";
                        messageBuilder.AppendLine($"<td>{categoryName}</td>");

                        messageBuilder.AppendLine($"<td>{product.Id}</td>");
                        var brandName = product.Brand != null ? product.Brand.Name : "N/A";
                        messageBuilder.AppendLine($"<td>{brandName}</td>");
                        messageBuilder.AppendLine($"<td>{product.Name}</td>");
                        messageBuilder.AppendLine($"<td>{product.Description}</td>");
                        messageBuilder.AppendLine($"<td>{cart.Quantity}</td>");
                        messageBuilder.AppendLine($"<td>{cart.Price:C}</td>");
                        messageBuilder.AppendLine($"<td>{(cart.Price * cart.Quantity):C}</td>");
                        messageBuilder.AppendLine("</tr>");
                    }
                }

                messageBuilder.AppendLine("</table>");

                // Gửi email với nội dung chi tiết sản phẩm dưới dạng bảng
                var receiver = userEmail;
                var subject = "Xác nhận đơn hàng";
                var message = messageBuilder.ToString();

                await _emailSender.SendEmailAsync(receiver, subject, message);

                HttpContext.Session.Remove("Cart");

                TempData["success"] = "Đơn hàng của bạn đã được tạo thành công! Vui lòng kiểm tra email để biết chi tiết.";

                return RedirectToAction("Index", "Cart");
            }
        }

    }
}
