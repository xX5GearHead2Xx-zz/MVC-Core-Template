using Ecommerce.DataModels;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.User
{
    public class AccountManagementController : BaseController
    {
        [Authorize]
        public IActionResult Account()
        {
            DataModels.User CurrentUser = new DataModels.User(UserID);
            return View(CurrentUser);
        }
    }
}
