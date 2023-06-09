using AspNetCoreIdentity.App.Core.ViewModels;
using AspNetCoreIdentity.App.Repository.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.App.Service.Services
{
    public interface IHomeService
    {
        Task<IdentityResult> CreateUserAsync(SignUpVM request);
        Task<IdentityResult> AddClaimToUserAsync(AppUser user, Claim claim);
    }
}
