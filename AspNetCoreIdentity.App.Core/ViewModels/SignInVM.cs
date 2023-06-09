using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class SignInVM
    {

       
        [Required(ErrorMessage = "Email field cannot be empty")]
        [EmailAddress(ErrorMessage = "Wrong Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember Me :")]
        public bool RememberMe { get; set; }
    }
}
