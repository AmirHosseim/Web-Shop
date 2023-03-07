using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWeb.Data;
using System.Security.Claims;
using UserServices.UserFavorite;
using UserServices.UserManager;
using UserServices.ViewModels;
using System.Net;


namespace ShopWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ShopWebContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ShopWebContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null)
            {
                return NotFound();
            }

            var favorits = _context.Favorites.Include(c => c.Category).Where(f => f.UserId == user.Id.ToString()).ToList();

            var model = new ShowAccountDetailsViewModel()
            {
                Favorites = favorits,
                User = user,
            };


            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogIn(string returnUrl = "")
        {
            if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            var model = new SignInViewModel()
            {
                returnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(SignInViewModel model)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (model.Password == null || model.Email == null)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "حسابی یافت نشد";
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, true);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ViewData["ErrorMessage"] = "به دلیل اشتباه وارد کردن متعدد رمز عبور اکانت شما به مدت 7 دقیقه قفل شده";
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "اکانتی با این مشخصاتد یافت نشد");

            return View(model);
        }


        [HttpPost]
        public IActionResult ExternalLogIn(string provider, string returnUrl = "")
        {
            var redirectUrl = Url.Action("ExternalLogInCallBack", "Account"
                , new { returnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);   
        }

        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = "", string remoteError = "")
        {
            returnUrl =
                (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");

            var loginViewModel = new SignInViewModel()
            {
                returnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error : {remoteError}");
                return View("Login", loginViewModel);
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
                return View("Login", loginViewModel);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, true, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var userName = email.Split('@')[0];
                    user = new IdentityUser()
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await _userManager.CreateAsync(user);
                }

                await _userManager.AddLoginAsync(user, externalLoginInfo);
                await _signInManager.SignInAsync(user, false);

                return Redirect(returnUrl);
            }

            ViewBag.ErrorTitle = "لطفا با بخش پشتیبانی تماس بگیرید";
            ViewBag.ErrorMessage = $"دریافت کرد {externalLoginInfo.LoginProvider} نمیتوان اطلاعاتی از";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddFavorite()
        {

            var model = new AddfavoriteViewModel()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            };

            var categories = _context.Categories.ToList();

            foreach (var item in categories)
            {
                if (!_context.Favorites.Any(f => f.UserId == model.UserId && f.CategoriId == item.Id))
                    model.CategoriesVm.Add(new CategoriesViewModel() { CategoryId = item.Id, CategoryName = item.Name });
            }

            return View(model);
        }

        public IActionResult AddFavorite(AddfavoriteViewModel model)
        {
            var SelectedCategories = model.CategoriesVm.Where(c => c.IsSelected).ToList();

            if (SelectedCategories.Count() > 0)
            {
                var user = _userManager.FindByIdAsync(model.UserId);
                if (user == null) return NotFound();

                foreach (var category in SelectedCategories)
                {
                    var favorite = new Favorite()
                    {
                        UserId = model.UserId,
                        CategoriId = category.CategoryId,
                    };

                    _context.Favorites.Add(favorite);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditAccount()
        {
            var user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null) return NotFound();

            var model = new EditUserViewModel()
            {
                UserName = user.Result.UserName,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditAccount(EditUserViewModel model)
        {
            if (model.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "لطفا نام کاربری را وارد کنید");
                return View(model);
            }

            var user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null) return NotFound();

            user.Result.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(await user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditPassword()
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null) return NotFound();

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ShowOrders()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var Orders = _context.Orders.Include(o => o.Orderdetails)
                .ThenInclude(p => p.product)
                    .Where(o => o.UserId == UserId).ToList();

            return View(Orders);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateSpecialUser()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(UserId)) return NotFound();

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null) return NotFound();

            var outTimeDays = new List<OutTimeDay>()
            {
                new OutTimeDay{Title = "یک ماه",Day=30,Price=20000},
                new OutTimeDay{Title = "سه ماه",Day=90,Price=60000},
                new OutTimeDay{Title = "نه ماه",Day=30,Price=180000},
                new OutTimeDay{Title = "یک سال",Day=30,Price=240000},
            };

            var model = new CreatSpecialUserViewModel()
            {
                UserId = user.Id,
                OutTimeDays = outTimeDays,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialUser(CreatSpecialUserViewModel model)
        {
            if (model.OutTimeDay == 0)
            {
                ModelState.AddModelError(string.Empty, "مشکلی رخ داد");

                var outTimeDays = new List<OutTimeDay>
                {
                    new OutTimeDay{Title = "یک ماه",Day=30,Price=20000},
                    new OutTimeDay{Title = "سه ماه",Day=90,Price=60000},
                    new OutTimeDay{Title = "نه ماه",Day=30,Price=180000},
                    new OutTimeDay{Title = "یک سال",Day=30,Price=240000},
                };

                model.OutTimeDays = outTimeDays;

                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null) return NotFound();

            var specialUser = new SpecialUser()
            {
                UserId = user.Id,
                CreatDate = DateTime.Now,
                OutTimeDay = model.OutTimeDay,
                IsFinished = false,
                FinishDateTime = DateTime.Now.AddDays(model.OutTimeDay),
            };

            _context.SpecialUsers.Add(specialUser);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

    }
}
