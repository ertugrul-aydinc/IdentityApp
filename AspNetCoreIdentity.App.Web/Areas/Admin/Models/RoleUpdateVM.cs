using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.App.Web.Areas.Admin.Models
{
    public class RoleUpdateVM
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Role name field cannot be empty")]
        [Display(Name = "Role Name :")]
        public string Name { get; set; } = null!;
    }
}
