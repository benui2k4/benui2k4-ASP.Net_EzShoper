using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUserModel> _userManager;

        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.OrderByDescending(r => r.Id).ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (!await _roleManager.RoleExistsAsync(model.Name))
            {

                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));

                if (result.Succeeded)
                {
                    TempData["success"] = "Thêm mới role thành công!";
                    return RedirectToAction("Index");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Role đã tồn tại.");
            }

            return View(model);
        }
        public async Task<IActionResult> Delete(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                TempData["error"] = "Role không tồn tại!";
                return RedirectToAction("Index");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["error"] = "Không thể xóa role này vì có người dùng đang sử dụng nó!";
                return RedirectToAction("Index");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["success"] = "Xóa role thành công!";
            }
            else
            {
                TempData["error"] = "Có lỗi phát sinh khi xóa role!";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id , IdentityRole model)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                TempData["error"] = "Role không tồn tại!";
                return RedirectToAction("Index");
            }

            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy role từ cơ sở dữ liệu
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                TempData["error"] = "Role không tồn tại!";
                return RedirectToAction("Index");
            }

            // Cập nhật thông tin role
            role.Name = model.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["success"] = "Cập nhật role thành công!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model); 
        }
    }
}
