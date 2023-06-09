using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class SignUpVM
    {
        [Required(ErrorMessage ="Username field cannot be empty")]
        [Display(Name ="Username :")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email field cannot be empty")]
        [EmailAddress(ErrorMessage = "Wrong Email")]
        [Display(Name = "Email :")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone field cannot be empty")]
        [Display(Name = "Phone :")]
        public string? Phone { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [Display(Name = "Password :")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Passwords are not matching")]
        [Required(ErrorMessage = "RePassword cannot be empty")]
        [Display(Name = "RePassword :")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? PasswordConfirm { get; set; }
    }
}
