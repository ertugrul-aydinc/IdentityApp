using AspNetCoreIdentity.App.Core.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Core.ViewModels
{
    public class UserEditVM
    {
        [Required(ErrorMessage = "Username field cannot be empty")]
        [Display(Name = "Username :")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email field cannot be empty")]
        [EmailAddress(ErrorMessage = "Wrong Email")]
        [Display(Name = "Email :")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone field cannot be empty")]
        [Display(Name = "Phone :")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "City :")]
        public string? City { get; set; }


        [Display(Name = "Profil Image")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Gender :")]
        public Gender? Gender { get; set; }

    }
}
