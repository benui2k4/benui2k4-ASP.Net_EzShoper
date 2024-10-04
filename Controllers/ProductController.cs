using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Controllers
{
	public class ProductController : Controller
	{
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public ActionResult Index() 
		{
			return View();
		}

        public async Task<IActionResult> Details(int Id )
        {
            if(Id == null ) return RedirectToAction("Index");
            var productsById = _dataContext.Products.Where(p=>p.Id == Id ).FirstOrDefault();
            return View(productsById);
        }
    }
}
