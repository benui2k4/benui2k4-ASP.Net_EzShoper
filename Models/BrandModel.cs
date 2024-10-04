using System.ComponentModel.DataAnnotations;

namespace ASP.Net_EzShoper.Models
{
	public class BrandModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Tên thương hiệu không được để trống !")]
		public string Name { get; set; }
		[Required( ErrorMessage = "Mô tả thương hiệu không được để trống!")]
		public string Description { get; set; }

		public string Slug { get; set; }

		public int Status { get; set; }
	}
}
