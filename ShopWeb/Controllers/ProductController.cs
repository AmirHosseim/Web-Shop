using ChannelServices.CategoriesServices;
using ChannelServices.ProductServices;
using ChannelServices.Repositories;
using ChannelServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWeb.Data;
using System.Security.Claims;

namespace ShopWeb.Controllers
{
    public class ProductController : Controller
    {
        private ShopWebContext _context;
        private IGetProducts _getProducts;
        public ProductController(ShopWebContext context, IGetProducts getProducts)
        {
            _context = context;
            _getProducts = getProducts;
        }

        [HttpGet]
        public IActionResult Index(string SearchString = "", int PageId = 1)
        {
            var model = new ShowProductsViewModel();

            var products = new List<Product>();

            if (SearchString == null)
            {
                products = _context.Products.Include(c => c.Channel)
                        .Include(i => i.ProductImages).ToList();
            }
            else
            {
                products = _context.Products.Include(c=> c.Channel)
                        .Include(i=> i.ProductImages).Where(p => p.Name.Contains(SearchString) ||
                    p.Description.Contains(SearchString) || p.Channel.Name.Contains(SearchString)).ToList();
            }

            model = _getProducts.ShowProductsViewModel(PageId, products);
            model.SearchString = SearchString;

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(string Id)
        {
            var product = _context.Products.Include(c => c.Category)
                .Include(c=> c.Channel).Include(i=> i.ProductImages).SingleOrDefault(p => p.Id == Id);

            if (product == null) return NotFound();

            var model = new ProductDetailViewModel()
            {
                Product = product,
                SameProducts = _context.Products.Include(c => c.Comments)
                            .Include(i=> i.ProductImages)
                                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                                    .OrderByDescending(c => c.Comments.Count()).Take(10).ToList(),
            };

            return View(model);
        }

        [Authorize]
        public IActionResult AddToOrder(string productId)
        {
            if (productId == null) return NotFound();

            var product = _context.Products.SingleOrDefault(p => p.Id == productId);

            if (product == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders.FirstOrDefault(o => o.UserId == UserId && !o.IsFinished);

            if (order == null)
            {
                order = new Order()
                {
                    UserId = UserId,
                    TotalPrice = 0,
                    IsFinished = false,
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            var orderDetail = _context.OrderDetails.SingleOrDefault(o => o.ProductId == productId && o.OrderId == order.Id);

            if (orderDetail == null)
            {
                orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    QuantityInStock = 1,
                    Price = product.Price,
                    ProductId = product.Id,
                };

                order.TotalPrice += orderDetail.Price;

                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
            }
            else
            {
                orderDetail.QuantityInStock++;

                orderDetail.Price = product.Price * orderDetail.QuantityInStock;
                order.TotalPrice += product.Price;

                _context.SaveChanges();
            }

            return RedirectToAction("ShowStock");
        }

        [Authorize]
        public IActionResult LessProductFromStock(long OrderDetailId)
        {
            if (OrderDetailId == 0) return NotFound();

            var orderDetail = _context.OrderDetails.Include(p => p.product).SingleOrDefault(o => o.Id == OrderDetailId);
            if (orderDetail == null) return NotFound();

            var order = _context.Orders.SingleOrDefault(o => o.Id == orderDetail.OrderId && !o.IsFinished);
            if (order == null) return NotFound();

            if (orderDetail.QuantityInStock > 1)
            {
                orderDetail.QuantityInStock -= 1;

                orderDetail.Price = orderDetail.QuantityInStock * orderDetail.product.Price;

                order.TotalPrice -= orderDetail.product.Price;
            }
            else
            {
                order.TotalPrice -= orderDetail.product.Price;

                _context.OrderDetails.Remove(orderDetail);
            }

            _context.SaveChanges();

            return RedirectToAction("ShowStock");
        }

        [Authorize]
        public IActionResult RemoveOrderDetail(long OrderDetailId)
        {
            if (OrderDetailId == 0) return NotFound();

            var orderDetail = _context.OrderDetails.SingleOrDefault(o => o.Id == OrderDetailId);
            if (orderDetail == null) return NotFound();

            var order = _context.Orders.SingleOrDefault(o => o.Id == orderDetail.OrderId && !o.IsFinished);
            if (order == null) return NotFound();

            order.TotalPrice -= orderDetail.Price;

            _context.Remove(orderDetail);
            _context.SaveChanges();

            return RedirectToAction("ShowStock");
        }


        [Authorize]
        [HttpGet]
        public IActionResult ShowStock()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _context.Orders.Include(o => o.Orderdetails).ThenInclude(p => p.product)
                .SingleOrDefault(o => o.UserId == userId && !o.IsFinished);

            if (order == null)
            {
                order = new Order()
                {
                    UserId = userId,
                    TotalPrice = 0,
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            return View(order);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddRate(string ProductId)
        {
            if (Equals(ProductId, string.Empty)) return NotFound();

            if (!_context.Products.Any(p => p.Id == ProductId)) return NotFound();

            var model = new AddRateViewModel()
            {
                RatePoans = _context.RatePoans.ToList(),
                ProductId = ProductId,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddRate(AddRateViewModel model)
        {
            if (model == null) return NotFound();

            var comment = new Comment()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ProductId = model.ProductId,
                Title = model.Title,
                Description = model.Description,
                RatePoan = model.RatePoan,
            };

            _context.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Details", new { Id = model.ProductId });
        }

        [Route("Popular")]
        [HttpGet]
        public IActionResult Popular()
        {
            var products = _context.Products.Include(i=> i.ProductImages).ToList();

            if (products.Count() != 0)
            {
                var populerProducts = new List<PopulerProductsViewModel>();

                foreach (var product in products)
                {
                    populerProducts.Add(new PopulerProductsViewModel
                    {
                        ProductId = product.Id,
                        SillQuantity = _context.OrderDetails.Include(o => o.order)
                               .Where(o => o.ProductId == product.Id && o.order.IsFinished)
                                     .Sum(o => o.QuantityInStock),

                        product = product,
                    });
                }

                var model = populerProducts.OrderByDescending(m => m.SillQuantity).TakeLast(30);

                return View(model);
            }

            ModelState.AddModelError("", "محصولی یافت نشد");

            return View();
        }

        public IActionResult CategoriesList()
        {
            var model = new Dictionary<Category, List<Product>>();

            var categories = _context.Categories.ToList();

            if (categories.Count() != 0)
            {
                foreach (var item in categories)
                {
                    model.Add(item, _context.Products.Where(p => p.CategoryId == item.Id).Take(5).ToList());
                }

                return View(model);
            }

            return NotFound();
        }

        public IActionResult Categories()
        {
            var model = new Dictionary<Category, List<Product>>();

            var categories = _context.Categories
                .Include(p => p.Products).ThenInclude(i=> i.ProductImages)
                    .Where(c => c.Products.Any()).ToList();

            foreach (var item in categories)
            {
                model.Add(item, item.Products.TakeLast(5).ToList());
            }

            return View(model);
        }

        public IActionResult CategoryProducts(int CategoryId, int PageId = 1, string SearchString = "")
        {
            var category = _context.Categories
                    .SingleOrDefault(c => c.Id == CategoryId);

            if (category == null) return NotFound();

            var model = new CategoryProductsViewModel();

            var products = _context.Products.Include(c=> c.Channel)
                .Include(i=> i.ProductImages).Where(p=> p.CategoryId == CategoryId).ToList();

            if (SearchString == null)
            {
                model.showProduct = _getProducts.ShowProductsViewModel(PageId,
                    products);
            }
            else
            {
                model.showProduct = _getProducts.ShowProductsViewModel(PageId,
                    products.Where(p => p.Name.Contains(SearchString)
                       || p.Description.Contains(SearchString)  
                           || p.Channel.Name.Contains(SearchString)).ToList());
            }

            model.CategoryName = category.Name;
            model.CategoryId = category.Id;

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckOut()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Orders.Include(o => o.Orderdetails)
                .FirstOrDefault(o => o.UserId == UserId && o.IsFinished == false);

            if (order == null) return NotFound();

            var model = new PaymentViewModel()
            {
                OrderId = order.Id,
                Value = order.TotalPrice,

            };

             return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut(PaymentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var order = _context.Orders.FirstOrDefault(o => o.Id == model.OrderId);

            if (order == null) return NotFound();

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string Id = (model.OrderId + UserId).ToString();

            var SettelmentReceipt = new SettelmentReceipt()
            {
                OrderId = order.Id,
                UserId = UserId,
                Value = model.Value,
                PaymentDateTime = DateTime.Now,
                Id = Id,
            };

            order.IsFinished = true;

            _context.SettelmentReceipts.Add(SettelmentReceipt);
            _context.SaveChanges();

            return RedirectToAction("ShowStock");
        }

    }
}
