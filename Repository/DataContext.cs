using ASP.Net_EzShoper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Repository
{
	public class DataContext : IdentityDbContext<AppUserModel>
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<CategoryModel> Categories { get; set; }
		public DbSet<BrandModel> Brands { get; set; }

		public DbSet<ProductModel> Products { get; set; }
		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<OrderDetailsModel> OrderDetails { get; set; }
		



	}
}
