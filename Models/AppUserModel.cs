using Microsoft.AspNetCore.Identity;

namespace ASP.Net_EzShoper.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation {  get; set; }
        public string RoleId    { get; set; }

    }
}
