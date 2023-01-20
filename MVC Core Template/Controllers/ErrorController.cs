using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
