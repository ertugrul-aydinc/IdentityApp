namespace AspNetCoreIdentity.App.Web.Areas.Admin.Models
{
    public class AssignRoleVM
    {
        public string RoleId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool Exists { get; set; }
    }
}
