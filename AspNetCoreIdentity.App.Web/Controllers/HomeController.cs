using AspNetCoreIdentity.App.Web.Extensions;
using AspNetCoreIdentity.App.Repository.Models;
using AspNetCoreIdentity.App.Service.Services;
using AspNetCoreIdentity.App.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AspNetCoreIdentity.App.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IHomeService homeService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _homeService = homeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var identityResult = await _homeService.CreateUserAsync(request);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }


            var claimResult = await _homeService.AddClaimToUserAsync((await _userManager.FindByNameAsync(request.UserName!))!, new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString()));

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            TempData["Message"] = "Sign Up has been Completed Successfully";
            return RedirectToAction(nameof(HomeController.SignUp));


        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signInVM, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(signInVM.Email!);

            if (hasUser is null)
            {
                ModelState.AddModelError(string.Empty, "Wrong Email or Password");
                return View();
            }


            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, signInVM.Password!, signInVM.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Try again 3 minutes later" });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { "Wrong email or password", $"Number of failed logins: {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
                return View();
            }

            
            if (hasUser.BirthDate.HasValue)
                await _signInManager.SignInWithClaimsAsync(hasUser, signInVM.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });

            return Redirect(returnUrl!); 

        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;



            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId is null || token is null)
                throw new Exception("An unexpected error occurred");

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser is null)
            {
                ModelState.AddModelError(string.Empty, "User was not found");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password!);

            if (result.Succeeded)
                TempData["Message"] = "Your password has been successfully reset";
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                return View();
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            var hasUser = await _userManager.FindByEmailAsync(forgotPasswordVM.Email!);

            if (hasUser is null)
            {
                TempData["SuccessMessage"] = "Password reset link has been sent to your email address.";
                return RedirectToAction(nameof(ForgotPassword));
            }


            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            // sample link
            // https://localhost:7089?userId=12345&token=asasfdsfsdfsd

            // Email Service
            await _emailService.SendResetPasswordEmailAsync(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Password reset link has been sent to your email address.";

            return RedirectToAction(nameof(ForgotPassword));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}