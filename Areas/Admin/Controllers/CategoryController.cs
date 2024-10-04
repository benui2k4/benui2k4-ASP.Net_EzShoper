using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;

        }


        public async Task<IActionResult> Index(int pg = 1)
        {
            List<CategoryModel> category = _dataContext.Categories.OrderByDescending(c => c.Id).ToList();


            const int pageSize = 10; 

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = category.Count(); 

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel category)
        {

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (!string.IsNullOrEmpty(category.Name))
            {
                category.Slug = category.Name.Replace(" ", "-");
            }
            var slug = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Slug == category.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Danh mục đã tồn tại với slug này.");
                return View(category);
            }
            _dataContext.Add(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm mới danh mục thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, CategoryModel category)
        {

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (!string.IsNullOrEmpty(category.Name))
            {
                category.Slug = category.Name.Replace(" ", "-");
            }
            var slug = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Slug == category.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Danh mục đã tồn tại với slug này.");
                return View(category);
            }
            _dataContext.Update(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int Id)
        {
            var category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                TempData["error"] = "Có lỗi đã phát sinh khi xóa danh mục!";
                return RedirectToAction("Index");
            }
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int Id)
        {
            var category = await _dataContext.Categories.FindAsync(Id);

            if (category == null)
            {
                TempData["error"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(category);
        }
    }

}
