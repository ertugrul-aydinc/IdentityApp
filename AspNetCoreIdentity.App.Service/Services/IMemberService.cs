using AspNetCoreIdentity.App.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.App.Service.Services
{
    public interface IMemberService
    {
        Task<UserVM> GetUserVMByUserName(string userName);
        Task LogOutAsync();
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        Task<UserEditVM> GetUserEditVMAsync(string userName);
        SelectList GetGenderSelectList();
        Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditVM request, string userName);
        List<ClaimVM> GetClaims(ClaimsPrincipal principal);
    }
}
