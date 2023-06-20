using ChannelServices.ChannelServices;
using ChannelServices.Following_Services;
using ChannelServices.ProductServices;
using ChannelServices.RecuritmentServices;
using ChannelServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWeb.Data;
using ShopWeb.Models.Sevices;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace ShopWeb.Controllers
{
    [Authorize]
    public class ChannelController : Controller
    {
        private ShopWebContext _context;
        private UserManager<IdentityUser> _userManager;
        private ISendEmail _sendEmail;

        public ChannelController(ShopWebContext context, UserManager<IdentityUser> userManager, ISendEmail sendEmail)
        {
            _context = context;
            _userManager = userManager;
            _sendEmail = sendEmail;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Channe and product Managment Actions

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChannelDetail(string Id)
        {
            if (Id == null) return NotFound();

            var channel = _context.Channels.Include(p => p.products).Include(f => f.followers).SingleOrDefault(c => c.Id == Id);

            return View(channel);
        }

        [HttpGet]
        [Route("MyChannelsList")]
        public IActionResult MyChannelsList()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Channel> channels = _context.Channels.Include(p => p.products).Where(c => c.CreatorId == UserId).ToList();

            return View(channels);
        }

        [HttpGet]
        public IActionResult MyChannelIncome(string id)
        {
            if (!CheckIsFinishedTimeOutSpecialUserAccount()) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var channel = _context.Channels.SingleOrDefault(c => c.Id == id && c.CreatorId == UserId);

            if (channel == null) return NotFound();

            var orderDetails = _context.OrderDetails.Include(o => o.order)
                    .ThenInclude(s => s.SettelmentReceipt).Include(p => p.product)
                        .Where(o => o.order.IsFinished == true && o.product.ChannelId == channel.Id).ToList();

            var model = new ChannelIncomeViewModel()
            {
                OrderDetails = orderDetails,
                ChannelIncome = orderDetails.Sum(o => o.Price),
                Orders = orderDetails.Select(o => o.order).ToList(),
                SettelmentReceipts = orderDetails.Select(o => o.order.SettelmentReceipt).ToList(),
            };

            List<SellsProduct> sellsProducts = new List<SellsProduct>();

            foreach (var product in _context.Products.Where(p => p.ChannelId == channel.Id).ToList())
            {
                sellsProducts.Add(new SellsProduct
                {
                    product = product,
                    SellQuantity = orderDetails.Where(o => o.ProductId == product.Id).Sum(o => o.QuantityInStock),
                    SellValue = orderDetails.Where(o => o.ProductId == product.Id).Sum(o => o.Price),
                    order = orderDetails.Where(o => o.ProductId == product.Id).Select(o => o.order).ToList(),
                });
            }

            model.SellsProducts = sellsProducts;

            return View(model);
        }


        [HttpGet]
        [Authorize]
        public IActionResult AddChannel()
        {
            var model = new AddChannelViewModel();

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                model.AllowToCreate = true;

                return View(model);
            }
            else
            {
                model.AllowToCreate = false;

                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddChannel(AddChannelViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                ModelState.AddModelError("", "مشکلی رخ داد");

                return View(model);
            }

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string id = Guid.NewGuid().ToString().Replace(" ", "_").Substring(0, 10);

            while (_context.Channels.Any(p => p.Id == id))
            {
                id = Guid.NewGuid().ToString().Replace(" ", "_").Substring(0, 10);
            }

            var Channel = new Channel()
            {
                Id = id,
                CreatorId = UserId,
                BioGraphy = model.BioGraphy,
                Name = model.Name,
            };

            _context.Channels.Add(Channel);

            if (Path.GetExtension(model.Avatar.FileName) != ".jpg")
            {
                ViewData["AvatarFormatError"] = "فرمت باید .jpg باشد";
                return View(model);
            }

            try
            {
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "ChannelsAvatars",
                     Channel.Id
                        + Path.GetExtension(model.Avatar.FileName));

                string Format = Path.GetExtension(model.Avatar.FileName);

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    model.Avatar.CopyTo(stream);
                }

                _context.SaveChanges();
            }
            catch
            {
                ViewData["ErrorInSaveAvatar"] = "مشکلی در ذخیره عکس پروفایل به وجود آمد";
                return View(model);
            }

            return RedirectToAction("MyChannelsList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrChangeChannelAvatar(string ChannelId, IFormFile Avatar)
        {
            var Channel = _context.Channels.SingleOrDefault(c =>
                c.Id == ChannelId);

            if (Channel == null) return NotFound();

            if (Path.GetExtension(Avatar.FileName) == ".jpg")
            {
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "ChannelsAvatars",
                     ChannelId
                        + Path.GetExtension(Avatar.FileName));

                string Format = Path.GetExtension(Avatar.FileName);

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    Avatar.CopyTo(stream);
                }
            }

            return RedirectToAction("ChannelManagment", new { ChannelId = ChannelId });
        }

        [HttpGet]
        public IActionResult AddProductToChannel(string channelId)
        {
            var channel = _context.Channels.SingleOrDefault(c => c.Id == channelId);

            if (channel == null) return NotFound();

            var model = new AddProductViewModel();

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!CheckAccessOrIsOwner("AddProduct", channel.Id)) return NotFound();

                model.ChannelId = channelId;
                model.AllowToAddOrEditProduct = true;
                model.Categories = _context.Categories.ToList();

                return View(model);
            }
            else
            {
                model.AllowToAddOrEditProduct = false;
                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductToChannel(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllowToAddOrEditProduct = CheckIsFinishedTimeOutSpecialUserAccount();
                model.Categories = _context.Categories.ToList();
                return View(model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var channel = _context.Channels.SingleOrDefault(c => c.Id == model.ChannelId);

            if (channel == null) return NotFound();

            if (!CheckAccessOrIsOwner("AddProduct", channel.Id)) return NotFound();

            string Id = Guid.NewGuid().ToString().Replace(" ", "_").Substring(1, 10);

            while (_context.Products.Any(p => p.Id == Id))
            {
                Id = Guid.NewGuid().ToString().Replace(" ", "_").Substring(1, 10);
            }

            var product = new Product()
            {
                Id = Id,
                Name = model.Name,
                Description = model.Description,
                ChannelId = model.ChannelId,
                CategoryId = model.CategoryId,
                Price = model.Price,
            };

            _context.Products.Add(product);

            string FileName = product.Id + " " + Id;

            string PictureId = Guid.NewGuid().ToString().Substring(1, 5).Replace(" ", "-");

            var ProductImage = new ProductImageInfo()
            {
                FileName = FileName,
                Format = Path.GetExtension(model.Picture.FileName),
                ProductId = product.Id,
                ImageId = PictureId,
            };

            _context.ProductsImagesInfo.Add(ProductImage);

            string FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "ProductsImages",
                ProductImage.FileName
                    + Path.GetExtension(model.Picture.FileName));

            string Format = Path.GetExtension(model.Picture.FileName);

            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                model.Picture.CopyTo(stream);
            }

            _context.SaveChanges();

            return RedirectToAction("ProductManagment", new { ChannelId = model.ChannelId });
        }

        [HttpGet]
        public IActionResult AddProductImage(string ProductId)
        {
            var product = _context.Products.Include(c => c.Channel).SingleOrDefault(p => p.Id == ProductId);

            if (product == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new AddProductImageViewModel();

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                if (!CheckAccessOrIsOwner("AddImageForPoroduct", product.ChannelId)) return NotFound();

                model.AddedImages = _context.ProductsImagesInfo.Where(p => p.ProductId == ProductId).ToList();
                model.ProductId = ProductId;
                model.IsAllow = true;

                return View(model);
            }
            else
            {
                model.IsAllow = false;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult AddProductImage(AddProductImageViewModel model)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == model.ProductId);

            if (product == null) return NotFound();

            if (!CheckAccessOrIsOwner("AddImageForPoroduct", product.ChannelId)) return NotFound();

            if (model.Image == null)
            {
                ModelState.AddModelError(String.Empty, "لطفا فایل را وارد کنید");
                model.IsAllow = true;
                return View(model);
            }


            string Id = Guid.NewGuid().ToString().Replace(" ", "-").Substring(1, 5);

            while (_context.ProductsImagesInfo.Any(p => p.ImageId == Id))
            {
                Id = Guid.NewGuid().ToString().Replace(" ", "_").Substring(1, 10);
            }

            var ImageInfo = new ProductImageInfo()
            {
                ProductId = model.ProductId,
                ImageId = Id,
                FileName = model.ProductId + " " + Id,
            };

            _context.ProductsImagesInfo.Add(ImageInfo);


            string FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "ProductsImages",
                model.ProductId + " " + Id
                    + Path.GetExtension(model.Image.FileName));

            string Format = Path.GetExtension(model.Image.FileName);

            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                model.Image.CopyTo(stream);
            }

            ImageInfo.Format = Format;
            _context.SaveChanges();



            return RedirectToAction("AddProductImage", new { ProductId = model.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProductImg(string ImgId, string ProductId)
        {
            if (ImgId == null || ProductId == null) return NotFound();

            if (!CheckIsFinishedTimeOutSpecialUserAccount()) return NotFound();

            var product = _context.Products.Include(c => c.Channel).FirstOrDefault(p => p.Id == ProductId);

            if (product == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!CheckAccessOrIsOwner("AddImageForPoroduct", product.ChannelId)) return NotFound();

            var ImgInfo = _context.ProductsImagesInfo
                    .SingleOrDefault(i => i.ImageId == ImgId && i.ProductId == ProductId);

            if (ImgInfo == null) return NotFound();

            _context.ProductsImagesInfo.Remove(ImgInfo);
            _context.SaveChanges();


            return RedirectToAction("AddProductImage", new { ProductId = ProductId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveProductFromChannel(string ProductId, string ChannelId)
        {
            if (ProductId == null || ChannelId == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!CheckIsFinishedTimeOutSpecialUserAccount()) return NotFound();

            var channel = _context.Channels.SingleOrDefault(c => c.Id == ChannelId);

            if (channel == null) return NotFound();

            if (!CheckAccessOrIsOwner("DeletProduct", channel.Id)) return NotFound();

            var product = _context.Products.SingleOrDefault(p => p.Id == ProductId && p.ChannelId == channel.Id);

            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("ChannelManagment", new { ChannelId = channel.Id });
        }

        [HttpGet]
        public IActionResult EditProduct(string Id)
        {
            var product = _context.Products.Include(c => c.Channel).SingleOrDefault(p => p.Id == Id);

            if (product == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new EditProductViewModel();

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                if (!CheckAccessOrIsOwner("EditProduct", product.ChannelId)) return NotFound();

                model.Id = product.Id;
                model.Name = product.Name;
                model.Description = product.Description;
                model.Price = product.Price;
                model.ChannelId = product.ChannelId;
                model.AllowToEditProduct = true;


                return View(model);
            }
            else
            {
                model.AllowToEditProduct = false;

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult EditProduct(EditProductViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                model.AllowToEditProduct = CheckIsFinishedTimeOutSpecialUserAccount();
                return View(model);
            }

            var product = _context.Products.SingleOrDefault(p => p.Id == model.Id);

            if (product == null) return NotFound();

            if (!CheckAccessOrIsOwner("EditProduct", product.ChannelId)) return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;

            _context.SaveChanges();

            return RedirectToAction("ChannelManagment", new { ChannelId = model.ChannelId });
        }

        [HttpGet]
        public IActionResult EditChannel(string ChannelId)
        {
            if (ChannelId == null) return NotFound();

            var model = new EditChannelViewModel();

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var channel = _context.Channels.SingleOrDefault(c => c.Id == ChannelId && c.CreatorId == UserId);

                if (channel == null) return NotFound();

                model.Name = channel.Name;
                model.BioGraphy = channel.BioGraphy;
                model.CreatorId = UserId;
                model.ChannelId = channel.Id;
                model.AlreadySpecialUserAccount = true;
            }
            else
            {
                model.AlreadySpecialUserAccount = false;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditChannel(EditChannelViewModel model)
        {
            if (!CheckIsFinishedTimeOutSpecialUserAccount()) return NotFound();

            else
            {
                model.AlreadySpecialUserAccount = true;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var channel = _context.Channels.SingleOrDefault(c => c.CreatorId == UserId && c.Id == model.ChannelId);

            if (channel == null) return NotFound();

            channel.Name = model.Name;
            channel.BioGraphy = model.BioGraphy;

            _context.SaveChanges();

            return RedirectToAction("ChannelManagment", new { ChannelId = channel.Id });
        }

        [HttpGet]
        public IActionResult ChannelManagment(string ChannelId)
        {
            if (String.IsNullOrEmpty(ChannelId)) return NotFound();

            string Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var channel = _context.Channels.Include(p => p.products)
                .ThenInclude(i => i.ProductImages).SingleOrDefault(c => c.Id == ChannelId);

            if (channel == null) return NotFound();

            var accessToEmploys = _context.AccessToEmploys.Include(c => c.Access)
                .Where(a => a.UserId == Userid && a.ChannelId == channel.Id).ToList();

            if (accessToEmploys == null && channel.CreatorId != Userid) return NotFound();

            var model = new ChannelManagmentViewModel()
            {
                Channel = channel,
                AccessToEmploys = accessToEmploys,
            };

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                model.AllowToManageChannel = true;
                return View(model);
            }
            else
            {
                model.AllowToManageChannel = false;
                ModelState.AddModelError(String.Empty, "شما حساب کاربری ویژه فعال ندارید");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult FollowOrUnfollowChannel(string ChannelId)
        {
            if (Equals(ChannelId, ""))
            {
                return NotFound();
            }

            var channel = _context.Channels.Include(p => p.followers).SingleOrDefault(c => c.Id == ChannelId);

            if (channel == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == channel.CreatorId) return NotFound();

            var follow = _context.Follows.SingleOrDefault(f => f.UserId == UserId && f.ChannelId == channel.Id);

            if (follow == null)
            {
                follow = new Follow()
                {
                    UserId = UserId,
                    ChannelId = channel.Id
                };

                _context.Follows.Add(follow);
            }
            else
            {
                _context.Follows.Remove(follow);
            }

            _context.SaveChanges();

            return RedirectToAction("ChannelDetail", new { Id = channel.Id });
        }

        #endregion

        #region RecuritmentInChannel Managment Actions

        [HttpGet]
        public IActionResult AddRecuritmentFormInChannel(string channelId)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                if (channelId == null) return NotFound();

                var channel = _context.Channels.SingleOrDefault(c => c.Id == channelId
                    && c.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (channel == null) return NotFound();

                var model = new AddRecuritmentFormViewModel()
                {
                    ChannelId = channel.Id,
                };

                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult AddRecuritmentFormInChannel(AddRecuritmentFormViewModel model)
        {
            if (!CheckIsFinishedTimeOutSpecialUserAccount()) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var channel = _context.Channels.SingleOrDefault(c => c.Id == model.ChannelId
                && c.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (channel == null) return NotFound();

            var RecuritmentForm = new RecruitmentInChannelForm()
            {
                Description = model.Description,
                ChannelId = model.ChannelId,
            };

            _context.RecruitmentInChannelForms.Add(RecuritmentForm);
            _context.SaveChanges();

            return RedirectToAction("ChannelManagment", new { ChannelId = channel.Id });
        }

        [HttpGet]
        public IActionResult ReplyToRecuritmentForm(int formId)
        {
            if (formId == 0) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var form = _context.RecruitmentInChannelForms.Include(c => c.channel).SingleOrDefault(f => f.Id == formId);

            if (form == null) return NotFound();

            if (form.channel.CreatorId == UserId) return NotFound();

            var model = new ReplyToRecuritmentFormViewModel();

            if (_context.ReplyToRecruitmentForms.Any(f => f.UserId == UserId))
            {
                ModelState.AddModelError("", "شما قبلا درخواست داده اید");
                model.AllowToReply = false;
                return View(model);
            }

            model.RecruitmenFormtId = form.Id;
            model.AllowToReply = true;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyToRecuritmentForm(ReplyToRecuritmentFormViewModel model)
        {
            if (model == null) return NotFound();

            var form = _context.RecruitmentInChannelForms.Include(c => c.channel).SingleOrDefault(f => f.Id == model.RecruitmenFormtId);

            if (form == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (form.channel.CreatorId == UserId) return NotFound();

            var user = await _userManager.FindByIdAsync(UserId);

            string UserName = user.UserName;

            var ReplyToform = new ReplyToRecruitmentForm()
            {
                UserId = UserId,
                RecruitmenFormtId = form.Id,
                Description = model.Description,
                Name = user.UserName,
                WasNotAccept = false,
            };

            _context.ReplyToRecruitmentForms.Add(ReplyToform);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyChannelRecuritmentForms")]
        public IActionResult MyChannelRecuritmentForms(string ChannelId)
        {
            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var channel = _context.Channels.SingleOrDefault(c => c.CreatorId == UserId && c.Id == ChannelId);

                if (channel == null) return NotFound();

                List<RecruitmentInChannelForm> forms =
                    _context.RecruitmentInChannelForms.Include(c => c.channel).Where(f => f.ChannelId == ChannelId).ToList();

                return View(forms);
            }
            else
            {
                return View("شما حساب ویژه فعال ندارید");
            }
        }

        [Route("RecuritmentForms")]
        public IActionResult RecuritmentFormsList()
        {
            IEnumerable<RecruitmentInChannelForm> forms = _context.RecruitmentInChannelForms
                .Include(c => c.channel).ThenInclude(f => f.followers)
                    .Where(f => f.WasHired == false).ToList();

            return View(forms);
        }

        [HttpGet]
        public IActionResult RecruitmentReplys(long FormId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var form = _context.RecruitmentInChannelForms.SingleOrDefault(c => c.Id == FormId);

            if (form == null) return NotFound();

            var channel = _context.Channels.SingleOrDefault(c => c.Id == form.ChannelId && c.CreatorId == userId);

            if (channel == null) return NotFound();

            var Replys = _context.ReplyToRecruitmentForms.Include(c => c.User).Where(r => r.RecruitmenFormtId == FormId).ToList();

            return View(Replys);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectToReply(long formId, string UserId)
        {
            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                var form = _context.RecruitmentInChannelForms.SingleOrDefault(f => f.Id == formId);

                if (form == null) return NotFound();

                var channel = _context.Channels.SingleOrDefault(c => c.Id == form.ChannelId
                    && c.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (channel == null) return NotFound();

                var Reply = _context.ReplyToRecruitmentForms.SingleOrDefault(r => r.UserId == UserId
                    && r.RecruitmenFormtId == formId);

                if (Reply == null) return NotFound();

                var user = await _userManager.FindByIdAsync(UserId);

                Reply.WasNotAccept = true;
                _context.SaveChanges();

                try
                {
                    string message = " کاربر گرامی درخواست استخدام شما برای فرم" + form.Description + " رد شد ";
                    string subject = "درخواست شما رد شد";
                    _sendEmail.sendEmail(user.Email, message, subject);
                }
                catch
                {
                    return RedirectToAction("RecruitmentReplys", new { FormId = form.Id });
                }

                return RedirectToAction("RecruitmentReplys", new { FormId = form.Id });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AceptToReply(int formId, string UserId)
        {
            if (CheckIsFinishedTimeOutSpecialUserAccount())
            {
                var user = await _userManager.FindByIdAsync(UserId);

                if (user == null) return NotFound();

                var form = _context.RecruitmentInChannelForms.SingleOrDefault(f => f.Id == formId);

                if (form == null) return NotFound();

                var channel = _context.Channels.SingleOrDefault(c => c.Id == form.ChannelId
                    && c.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (channel == null) return NotFound();

                var accessSelecrion = new List<AccessSelection>();

                foreach (var item in _context.Accesses.ToList())
                {
                    accessSelecrion.Add(new AccessSelection() { access = item, IsSelect = false });
                }

                var model = new AcceptToReplyViewModel()
                {
                    ChannelId = channel.Id,
                    UserId = UserId,
                    AccessSelections = accessSelecrion,
                    formId = formId,
                };

                return View(model);

            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AceptToReply(AcceptToReplyViewModel model)
        {
            var selectedAccess = model.AccessSelections.Where(a => a.IsSelect == true).ToList();

            foreach (var item in selectedAccess)
            {
                _context.AccessToEmploys.Add(new AccessToEmploy()
                {
                    AccessId = item.access.Id,
                    ChannelId = model.ChannelId,
                    UserId = model.UserId,
                });
            }

            var form = _context.RecruitmentInChannelForms.SingleOrDefault(f => f.Id == model.formId);

            if (form == null) return NotFound();

            form.WasHired = true;

            var recruitmentInChannel = new RecruitmentInChannel()
            {
                ChannelId = model.ChannelId,
                RecuitmentFormId = form.Id,
                UserId = model.UserId,
            };

            _context.RecruitmentInChannels.Add(recruitmentInChannel);

            _context.SaveChanges();

            return RedirectToAction("MyChannelRecuritmentForms", new { ChannelId = model.ChannelId });
        }

        [HttpGet]
        [Route("MyRecruitmentChannels")]
        public IActionResult MyRecruitmentChannels()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = _context.RecruitmentInChannels.Include(c => c.channel).Where(r => r.UserId == UserId).ToList();

            return View(model);
        }

        #endregion

        #region Methods
        public bool CheckIsFinishedTimeOutSpecialUserAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var specialUser = _context.SpecialUsers.FirstOrDefault(
                    u => u.UserId == userId && !u.IsFinished);

            if (specialUser == null) return false;

            //if Time out has not finished
            if (DateTime.Now < specialUser.FinishDateTime)
            {
                return true;
            }
            else
            {
                if (!specialUser.IsFinished)
                {
                    specialUser.IsFinished = true;

                    _context.SaveChanges();
                }

                return false;
            }
        }

        public bool CheckAccessOrIsOwner(string AccessName, string ChannelId)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var channel = _context.Channels.SingleOrDefault(c => c.Id == ChannelId);

            if (channel == null) return false;

            if (!_context.AccessToEmploys.Any(a => a.ChannelId == channel.Id
                       && a.UserId == UserId && a.Access.Name == AccessName)
                           && channel.CreatorId != UserId) return false;
            else
            {
                return true;
            }
        }

        #endregion
    }
}