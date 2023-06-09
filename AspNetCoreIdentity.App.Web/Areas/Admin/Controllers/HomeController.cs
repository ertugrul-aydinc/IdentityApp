using AspNetCoreIdentity.App.Web.Areas.Admin.Models;
using AspNetCoreIdentity.App.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.App.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();

            var userVMLİst = userList.Select(x => new UserVM()
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.UserName
            }).ToList();

            return View(userVMLİst);
        }
    }
}
