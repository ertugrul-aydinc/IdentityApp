using AspNetCoreIdentity.App.Web.Areas.Admin.Models;
using AspNetCoreIdentity.App.Web.Extensions;
using AspNetCoreIdentity.App.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.App.Web.Areas.Admin.Controllers
{

    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "admin,role-action")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleVM()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(roles);
        }


        [Authorize(Roles ="role-action")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleCreateVM request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if(!result.Succeeded) 
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["Message"] = "Role added successfully";
            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "role-action")]
        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate is null)
                throw new Exception("No role found to update.");

            return View(new RoleUpdateVM() { Id = roleToUpdate.Id, Name = roleToUpdate.Name!});
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleUpdateVM request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate is null)
                throw new Exception("No role found to update.");

            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);

            TempData["Message"] = "Role updated successfully";
            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "role-action")]
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete is null)
                throw new Exception("No role found to delete.");

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if(!result.Succeeded)
                throw new Exception(result.Errors.Select(x => x.Description).First());

            TempData["Message"] = "Role deleted successfully";
            return RedirectToAction(nameof(RolesController.Index));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!;
            ViewBag.userId = id;
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var roles = await _roleManager.Roles.ToListAsync();

            var roleVMList = new List<AssignRoleVM>();

            foreach (var role in roles)
            {
                var assignRoleVM = new AssignRoleVM() { RoleId = role.Id, Name = role.Name!};

                if(userRoles.Contains(role.Name!))
                    assignRoleVM.Exists = true;

                roleVMList.Add(assignRoleVM);
            }

            return View(roleVMList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId,List<AssignRoleVM> requestList)
        {
            var userToAssignRole = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in requestList)
            {
                if (role.Exists)
                    await _userManager.AddToRoleAsync(userToAssignRole, role.Name);
                else
                    await _userManager.RemoveFromRoleAsync(userToAssignRole, role.Name);
            }

            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
