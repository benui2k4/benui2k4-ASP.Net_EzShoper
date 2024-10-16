using System.ComponentModel.DataAnnotations;

namespace ASP.Net_EzShoper.Models
{
	public class UserModel
	{
		public int Id { get; set; }


		[Required(ErrorMessage ="Username không được để trống!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email không được để trống!"), EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber không được để trống!")]
        public string PhoneNumber { get; set; }

		[DataType(DataType.Password),Required(ErrorMessage ="Password không được để trống!")]
		public string Password { get; set; }
	}
}
