using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Routing.Constraints;
using Newtonsoft.Json.Serialization;

namespace ASP.Net_EzShoper.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }


		[Required( ErrorMessage = "Tên sản phẩm không được để trống!")]
		public string Name { get; set; }
		[Required( ErrorMessage = "Mô tả sản phẩm không được để trống!")]
		public string Description { get; set; }

        
        [Required(ErrorMessage = "Giá sản phẩm không được để trống!")]
        [Range(0.01,double.MaxValue)]
        //[Column(TypeName ="decimal(8,2)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

		public string Slug { get; set; }

        [Required,Range(1,int.MaxValue,ErrorMessage = "Chọn một danh mục sản phẩm! ")]
        public int CategoryId	{ get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu sản phẩm! ")]
        public int BrandId	{ get; set; }

        
        public CategoryModel Category { get; set; }
        
        public BrandModel Brand { get; set; }

		public string Image { get; set; } 
		[NotMapped]
        //[Required(ErrorMessage = "Hãy thêm 1 ảnh sản phẩm!")]
        //[FileExtensions(Extensions ="jpg,jpeg,png,gif",ErrorMessage ="Hãy chọn ảnh dạng jpg,jpeg,png,gif")]
       

        public IFormFile? ImageUpload { get; set; }
	}
}	
