using ASP.Net_EzShoper.Areas.Admin.Repository;
using ASP.Net_EzShoper.Models;
using ASP.Net_EzShoper.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);


//Connection Database ! Chuỗi kết nối
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// đăng kí services Send Email 
builder.Services.AddTransient<IEmailSender, EmailSender>();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential= true;

});

// khai báo identity 
builder.Services.AddIdentity<AppUserModel,IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
   

   
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

// view trang 404 
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");


app.UseSession();
var supportedCultures = new[] { new CultureInfo("en-US") }; // Sử dụng en-US để đồng bộ dấu .
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0].Name) // Đặt en-US làm mặc định
    .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
    .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();


// xác thực trên , dưới  phân quyền
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
	name: "Areas",
	pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");


// customer route Category bên User 

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new { controller = "Category", action = "Index" });

// customer route Brand bên User 
app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

SeedData.SeedingData(context);


app.Run();
