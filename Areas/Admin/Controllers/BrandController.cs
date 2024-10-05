using ASP.Net_EzShoper.Controllers;
using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;

        }


        public async Task<IActionResult> Index(int pg = 1)
        {
            //return View(await _dataContext.Brands.OrderByDescending(c => c.Id).ToListAsync());
            List<BrandModel> brands = _dataContext.Brands.OrderByDescending(b => b.Id).ToList();

            const int pageSize = 10; // Số lượng bản ghi mỗi trang

            if (pg < 1)
            {
                pg = 1; // Đảm bảo trang không nhỏ hơn 1
            }

            int recsCount = brands.Count(); // Tổng số bản ghi

            var pager = new Paginate(recsCount, pg, pageSize); // Khởi tạo đối tượng phân trang

            int recSkip = (pg - 1) * pageSize; // Tính số bản ghi bỏ qua

            var data = brands.Skip(recSkip).Take(pager.PageSize).ToList(); // Lấy dữ liệu cho trang hiện tại

            ViewBag.Pager = pager; // Truyền thông tin phân trang cho view

            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandModel brand)
        {

            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            if (!string.IsNullOrEmpty(brand.Name))
            {
                brand.Slug = brand.Name.Replace(" ", "-");
            }
            var slug = await _dataContext.Brands.FirstOrDefaultAsync(c => c.Slug == brand.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Thương hiệu đã tồn tại với slug này.");
                return View(brand);
            }
            _dataContext.Add(brand);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm mới thương hiệu thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, BrandModel brand)
        {

            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            if (!string.IsNullOrEmpty(brand.Name))
            {
                brand.Slug = brand.Name.Replace(" ", "-");
            }
            var slug = await _dataContext.Brands.FirstOrDefaultAsync(c => c.Slug == brand.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Thương hiệu đã tồn tại với slug này.");
                return View(brand);
            }
            _dataContext.Update(brand);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật thương hiệu thành công!";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int Id)
        {
            var brand = await _dataContext.Brands.FindAsync(Id);
            if (brand == null)
            {
                TempData["error"] = "Có lỗi đã phát sinh khi xóa thương hiệu!";
                return RedirectToAction("Index");
            }
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa thương hiệu thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int Id)
        {
            var brand = await _dataContext.Brands.FindAsync(Id);

            if (brand == null)
            {
                TempData["error"] = "Thương hiệu không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(brand);
        }
    }

}
