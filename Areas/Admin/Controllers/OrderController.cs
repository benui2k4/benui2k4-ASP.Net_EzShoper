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
        public async Task<IActionResult> Index(int pg =1 )
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
       
    }
}

