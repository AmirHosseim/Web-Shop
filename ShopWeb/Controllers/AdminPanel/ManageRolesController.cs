using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleServices.ViewModels;

namespace ShopWeb.Controllers.AdminPanel
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;

        public ManageRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();

            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(AddOrEditRoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ViewData["ErrorMessage"] = "لطفا نام مقام را وارد کنید";
                return View(model);
            }

            var role = new IdentityRole(model.Name);

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded) return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletRole(string RoleId)
        {
            if (RoleId == null) return NotFound();

            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role == null) return NotFound();

            await _roleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string RoleId)
        {
            if (string.IsNullOrEmpty(RoleId)) return NotFound();

            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role == null) return NotFound();

            var model = new AddOrEditRoleViewModel()
            {
                Name = role.Name,
                RoleId = role.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(AddOrEditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "لطفا فیلد را پر کنید");
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
                return NotFound();

            role.Name = model.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}

