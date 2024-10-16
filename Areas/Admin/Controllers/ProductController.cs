using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {


            const int pageSize = 10; 

            
            List<ProductModel> products = await _dataContext.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();

            
            if (pg < 1)
            {
                pg = 1;
            }

            
            int recsCount = products.Count();

            
            var pager = new Paginate(recsCount, pg, pageSize);

            
            int recSkip = (pg - 1) * pageSize;

            
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();

            
            ViewBag.Pager = pager;

            
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel product)
        {
            
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            // Nếu ModelState không hợp lệ, trả lại view với dữ liệu hiện tại
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Tạo slug nếu Name không rỗng
            if (!string.IsNullOrEmpty(product.Name))
            {
                product.Slug = product.Name.Replace(" ", "-");
            }

            // Kiểm tra slug trùng lặp
            var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Sản phẩm đã tồn tại với slug này.");
                return View(product); // Trả lại view nếu có lỗi
            }

            // Xử lý ảnh tải lên
            if (product.ImageUpload != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, imageName);

                // Sử dụng "using" để tự động đóng FileStream
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageUpload.CopyToAsync(fs);
                }

                product.Image = imageName;
            }

            // Lưu sản phẩm vào database
            _dataContext.Add(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm mới sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);


            var existed_product = await _dataContext.Products.FindAsync(Id);
            if (existed_product == null)
            {
                TempData["error"] = "Có lỗi phát sinh trong quá trình cập nhật sản phẩm!";
                return RedirectToAction("Index");
            }


            product.Slug = product.Name.Replace(" ", "-");
            var slug = await _dataContext.Products
                .FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != Id);
            if (slug != null)
            {
                ModelState.AddModelError("", "Sản phẩm đã tồn tại với Slug này!");
                return View(product);
            }

            // Xử lý hình ảnh mới nếu có tải lên
            if (product.ImageUpload != null)
            {
                // Xóa hình ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existed_product.Image))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/products", existed_product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }


                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    existed_product.Image = imageName;
                }
            }


            existed_product.Name = product.Name;
            existed_product.Description = product.Description;
            existed_product.Price = product.Price;
            existed_product.CategoryId = product.CategoryId;
            existed_product.BrandId = product.BrandId;


            _dataContext.Update(existed_product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {

            var product = await _dataContext.Products.FindAsync(Id);
            if (product == null)
            {
                TempData["error"] = "Có lỗi đã phát sinh khi xóa sản phẩm!";
                return RedirectToAction("Index");
            }


            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }
    }
}

