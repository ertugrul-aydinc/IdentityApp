using AspNetCoreIdentity.App.Core.Models;
using AspNetCoreIdentity.App.Core.ViewModels;
using AspNetCoreIdentity.App.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.App.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserVM> GetUserVMByUserName(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserVM
            {
                Email = currentUser!.Email,
                UserName = currentUser!.UserName,
                PhoneNumber = currentUser!.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            return await _userManager.CheckPasswordAsync(currentUser, password);
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
                return (false, resultChangePassword.Errors);

            await _userManager.UpdateSecurityStampAsync(currentUser);

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);

            return (true, null);
        }

        public async Task<UserEditVM> GetUserEditVMAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            return new UserEditVM()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
        }

        public SelectList GetGenderSelectList() => new SelectList(Enum.GetNames(typeof(Gender)));

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditVM request, string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City is null ? null : request.City.ToLower();
            currentUser.Gender = request.Gender;
            currentUser.PhoneNumber = request.Phone;


            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;

            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
                return (false, updateToUserResult.Errors);

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });
            else
                await _signInManager.SignInAsync(currentUser, true);

            return (true, null);
        }

        public List<ClaimVM> GetClaims(ClaimsPrincipal principal)
        {
            return principal.Claims.Select(c => new ClaimVM()
            {
                Issuer = c.Issuer,
                Type = c.Type,
                Value = c.Value
            }).ToList();
        }
    }
}
