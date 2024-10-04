using ASP.Net_EzShoper.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_EzShoper.Repository
{
	public class SeedData { 
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any()){
				CategoryModel macbook = new CategoryModel {Name= "macbook", Slug= "macbook", Description= "macbook is Large Brand in the world", Status=1 };
				CategoryModel pc = new CategoryModel {Name="pc" ,Slug="pc",Description="pc is Large Brand in the world",Status=1 };

				BrandModel dell = new BrandModel { Name = "dell", Slug = "dell", Description = "dell is Large Brand in the world", Status = 1 };
				BrandModel samsung = new BrandModel { Name = "samsung", Slug = "samsung", Description = "Samsung is Large Brand in the world", Status = 1 };
				_context.Products.AddRange(
					new ProductModel { Name = "macbook", Slug = "macbook", Description = "Iphone never die", Image = "1.jpg", Category = macbook, Price = 12000, Brand = dell },
					new ProductModel { Name = "pc", Slug = "pc", Description = "pc never die", Image = "2.jpg", Category = pc, Price = 12000, Brand = samsung }
				);
				_context.SaveChanges();
				
			}
		}
	}
}
