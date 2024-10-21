using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.Net_EzShoper.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên người đánh giá không được để trống!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ email không được để trống!")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống!")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Đánh giá sao không được để trống!")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao!")]
        public int Star { get; set; }  


        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
        public int ProductId { get; set; }
    }
}
