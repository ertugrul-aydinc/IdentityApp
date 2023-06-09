using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class PasswordChangeVM
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [Display(Name = "Current Password :")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string PasswordOld { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty")]
        [Display(Name = "New Password :")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string PasswordNew { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Passwords are not matching")]
        [Required(ErrorMessage = "RePassword cannot be empty")]
        [Display(Name = "RePassword :")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string PasswordNewConfirm { get; set; } = null!;
    }
}
