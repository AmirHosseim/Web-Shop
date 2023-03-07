using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleServices.ViewModels;

namespace ShopWeb.Controllers.AdminPanel
{
    [Authorize(Roles = "Admin")]
    public class ManageUserController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManageUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var user = _userManager.Users.ToList();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole(string UserId)
        {
            if (UserId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(UserId.ToString());

            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();

            var model = new AddToRoleOrRemoveViewModel()
            {
                UserId = UserId,
            };

            foreach (var role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.UserRoles.Add(new UserRolesViewModel { RoleName = role.Name });
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(AddToRoleOrRemoveViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var requestRoles = model.UserRoles.Where(u => u.IsSelected).Select(u => u.RoleName).ToList();

            var result = await _userManager.AddToRolesAsync(user, requestRoles);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveUserFromRole(string UserId)
        {
            if (UserId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            var model = new AddToRoleOrRemoveViewModel()
            {
                UserId = UserId,
            };

            var roles = _roleManager.Roles.ToList();

            foreach (var role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.UserRoles.Add(new UserRolesViewModel()
                    {
                        RoleName = role.Name,
                    });
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromRole(AddToRoleOrRemoveViewModel model)
        {
            if (model == null) return NotFound();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var requestRoles = model.UserRoles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

            if (!requestRoles.Any()) return NotFound();

            var result = await _userManager.RemoveFromRolesAsync(user, requestRoles);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(model);

        }
    }
}
