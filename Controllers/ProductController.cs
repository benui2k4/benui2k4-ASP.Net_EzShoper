using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Models.ViewModels;
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

        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return RedirectToAction("Index");

            var productsById = _dataContext.Products
                .Include(r => r.Rating)
                .Where(p => p.Id == Id).FirstOrDefault();
            if (productsById == null) return NotFound();

            var relatedProducts = await _dataContext.Products
                .Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
                .Take(3)
                .ToListAsync();

            var viewModel = new ProductDetailsRatingViewModel
            {
                ProductDetails = productsById,
                RatingDetails = productsById.Rating
            };



            ViewBag.RelatedProducts = relatedProducts;
            return View(viewModel);
        }


        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
            ViewBag.Keyword = searchTerm;
            return View(products);
        }

        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    EmailAddress = rating.EmailAddress,
                    Comment = rating.Comment,
                    Star = rating.Star,
                };

                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm mới đánh giá thành công !";

                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Không thể thêm mới đánh giá !";
                return RedirectToAction("Details", new { id = rating.ProductId} );

            }

        }
    }
}
