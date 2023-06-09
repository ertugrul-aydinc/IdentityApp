using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email field cannot be empty")]
        [EmailAddress(ErrorMessage = "Wrong Email")]
        [Display(Name = "Email :")]
        public string? Email { get; set; }
    }
}
