using AspNetCoreIdentity.App.Web.Extensions;
using AspNetCoreIdentity.App.Repository.Models;
using AspNetCoreIdentity.App.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCoreIdentity.App.Core.Models;
using AspNetCoreIdentity.App.Service.Services;

namespace AspNetCoreIdentity.App.Web.Controllers
{

    [Authorize]
    public class MembersController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private string userName => User.Identity!.Name!;
        private readonly IMemberService _memberService;

        public MembersController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMemberService memberService, IFileProvider fileProvider = null!)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
            => View(await _memberService.GetUserVMByUserName(userName));


        public async Task LogOut()
        {
            await _memberService.LogOutAsync();
        }


        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _memberService.CheckPasswordAsync(userName, request.PasswordOld))
            {
                ModelState.AddModelError(string.Empty, "Old password incorrect");
                return View();
            }


            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.PasswordOld, request.PasswordNew);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

           
            TempData["Message"] = "Your password changed successfully";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList();
            
            return View(await _memberService.GetUserEditVMAsync(userName));
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditVM request)
        {
            if(!ModelState.IsValid) return View();


            var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

            TempData["Message"] = "User infos changed successfully";

            return View(await _memberService.GetUserEditVMAsync(userName));

        }


        [HttpGet]
        public IActionResult Claims() => View(_memberService.GetClaims(User));


        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }


        [Authorize(Policy = "ViolancePolicy")]
        [HttpGet]
        public IActionResult ViolancePage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            string message = string.Empty;

            message = "You are not authorized to view this page";
            ViewBag.Message = message;

            return View();
        }
    }
}
