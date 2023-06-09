using AspNetCoreIdentity.App.Core.ViewModels;
using AspNetCoreIdentity.App.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.App.Service.Services
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddClaimToUserAsync(AppUser user, Claim claim)
        {
            var exchangeExpireClaim = claim;
            var _user = await _userManager.FindByNameAsync(user.UserName!);
            return await _userManager.AddClaimAsync(_user!, exchangeExpireClaim);
        }

        public async Task<IdentityResult> CreateUserAsync(SignUpVM request)
        {
            return await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email
            }, request.Password!);
        }
    }
}
