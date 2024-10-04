using System.ComponentModel.DataAnnotations;

namespace ASP.Net_EzShoper.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Username không được để trống!")]
        public string Username { get; set; }
       
        [DataType(DataType.Password), Required(ErrorMessage = "Password không được để trống!")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
