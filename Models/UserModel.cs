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

        [Required(ErrorMessage = "PhoneNumber không được để trống !")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Vui lòng nhập đúng định dạng Phone !")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password không được để trống!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái, một số và một ký tự đặc biệt.")]
        public string Password { get; set; }
    }
}
