using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;

        }
        public async Task<IActionResult> Index(int pg = 1)
        {

            List<OrderModel> orders = _dataContext.Orders.OrderByDescending(o => o.Id).ToList();

            const int pageSize = 10; // 10 items/trang

            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = orders.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = orders.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            // Kiểm tra ordercode có null hay không
            if (string.IsNullOrEmpty(ordercode))
            {
                return NotFound("Order code is required.");
            }

            var DetailsOrder = await _dataContext.OrderDetails
                .Include(od => od.Product) // Đảm bảo lấy được sản phẩm liên quan
                .Where(od => od.OrderCode == ordercode)
                .ToListAsync();

            // Kiểm tra nếu không có sản phẩm nào
            if (DetailsOrder == null || !DetailsOrder.Any())
            {
                return NotFound("No order details found for this order code.");
            }

            return View(DetailsOrder);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully ! " });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error occurred while updating the order status ! ");
            }


        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);

            if (order == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            var orderDetails = _dataContext.OrderDetails.Where(od => od.OrderCode == order.OrderCode);
            _dataContext.OrderDetails.RemoveRange(orderDetails);
            _dataContext.Orders.Remove(order);

            try
            {
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Xóa đơn hàng thành công!";
            }
            catch (Exception)
            {
                TempData["error"] = "Có lỗi phát sinh khi xóa đơn hàng!";
            }

            return RedirectToAction("Index");
        }


    }
}

