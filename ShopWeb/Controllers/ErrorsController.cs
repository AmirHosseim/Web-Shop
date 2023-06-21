using Microsoft.AspNetCore.Mvc;

namespace ShopWeb.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }
    }
}
