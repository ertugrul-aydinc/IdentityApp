using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class ResetPasswordVM
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [Display(Name = "New Password :")]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not matching")]
        [Required(ErrorMessage = "RePassword cannot be empty")]
        [Display(Name = "RePassword :")]
        public string? PasswordConfirm { get; set; }
    }
}
