using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Legal(Enums.LegalTextType TextType = Enums.LegalTextType.TermsAndConditions)
        {
            ViewBag.TextType = TextType;
            return View("~/Views/Legal/Legal.cshtml");
        }
    }
}
