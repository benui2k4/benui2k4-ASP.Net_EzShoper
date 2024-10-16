using ASP.Net_EzShoper.Areas.Admin.Repository;
using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ASP.Net_EzShoper.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(LoginVM.Username, LoginVM.Password, false, false);
                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng nhập thành công !";

                    var receiver = "phamnhusondolla@gmail.com";

                    var subject = "Đăng nhập trên thiết bị thành công !";

                    var message = "Đăng nhập thành công , trải nghiệm dịch vụ nhé !";

                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    return Redirect(LoginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác !");
            }
            return View(LoginVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel userModel = new AppUserModel
                {
                    UserName = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                IdentityResult result = await _userManager.CreateAsync(userModel, user.Password);
                if (result.Succeeded)
                {
                    
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    
                    var userRole = await _userManager.AddToRoleAsync(userModel, "User");
                    if (userRole.Succeeded)
                    {
                        var role = await _roleManager.FindByNameAsync("User");
                        if (role != null)
                        {
                            userModel.RoleId = role.Id;
                            await _userManager.UpdateAsync(userModel);
                        }

                        TempData["success"] = "Đăng kí thành công !";
                        return Redirect("/account/login");
                    }
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }


        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
