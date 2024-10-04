using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ASP.Net_EzShoper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class UserController : Controller

    {
        private readonly UserManager<AppUserModel> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly DataContext _dataContext;


        public UserController(DataContext dataContext, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }
        [Route("Index")]
        public async Task<ActionResult> Index(int pg = 1)
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        orderby u.Id descending 
                                        select new
                                        {
                                            User = u,
                                            RoleName = r.Name
                                        }).ToListAsync();

            const int pageSize = 10; 

            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = usersWithRoles.Count();

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = usersWithRoles.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<ActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(new AppUserModel());
        }
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);

                    var userId = createUser.Id;

                    var role = await _roleManager.FindByIdAsync(user.RoleId);
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    TempData["Success"] = "Thêm mới người dùng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                TempData["error"] = "Có lỗi khi thêm mới người dùng!";
                return RedirectToAction("Index", "User");
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Không tồn tại người dùng!";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["error"] = "Không tìm thấy người dùng!";
                return RedirectToAction("Index");
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                TempData["error"] = "Xóa người dùng thất bại!";
                return View("Error");
            }
            TempData["success"] = "Xóa người dùng thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Không tồn tại người dùng!";
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["error"] = "Không tìm thấy người dùng!";
                return View(user);
            }

            var roles = await _roleManager.Roles.ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }
        [HttpPost]
        [Route("Edit")]
        public async Task<ActionResult> Edit(string id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                TempData["error"] = "Không tìm thấy người dùng!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleId = user.RoleId;


                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    TempData["success"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in updateUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }


            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");


            return View(user);
        }

    }

}




