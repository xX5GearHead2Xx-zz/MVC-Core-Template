using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.ViewModels;
using Ecommerce.DataModels;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Ecommerce.Controllers
{
    public class AccountController : BaseController
    {
        [Authorize]
        public IActionResult Account()
        {
            DataModels.User CurrentUser = new DataModels.User(UserID);
            return View(CurrentUser);
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(IFormCollection Details)
        {
            if (await ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                Emails.TemplateBuilder Template = new Emails.TemplateBuilder(Enums.EmailTemplate.Contact);
                Template.ReplacePlaceholder("{ClientName}", Details["Name"].ToString());
                Template.ReplacePlaceholder("{ClientEmail}", Details["Email"].ToString());
                Template.ReplacePlaceholder("{ClientMessage}", Details["Message"].ToString());
                Azure.Communication.SendEmail("Contact Request", Template.Body.ToString(), new List<string>() { Configuration.configuration["EmailSettings:SupportEmail"].ToString() });
                AddNotification(new Notification("Sent", "Your message was sent successfully", Enums.NotificationType.Success));
            }
            else
            {
                AddNotification(new Notification("Invalid Recaptcha", "Please complete the recaptcha", Enums.NotificationType.Error));
            }
            return RedirectToAction("Contact");
        }

        public IActionResult Registration()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(IFormCollection Details)
        {
            if (await ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                DataModels.User NewUser = new DataModels.User();
                NewUser.Email = Details["Email"].ToString();
                if (!NewUser.Exists())
                {
                    NewUser.Salt = Security.CreateSalt();
                    NewUser.Password = Security.HashPassword(Details["Password"].ToString(), NewUser.Salt);
                    NewUser.Save();

                    await SignUserIn(NewUser.Key);

                    AddNotification(new Notification("Welcome", "Welcome " + NewUser.Email, Enums.NotificationType.Success));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddNotification(new Notification("Error", "The email entered is already registered", Enums.NotificationType.Error));
                    return RedirectToAction("Registration");
                }
            }
            else
            {
                AddNotification(new Notification("Error", "TInvalid Recaptcha", Enums.NotificationType.Error));
                return RedirectToAction("Registration");
            }

        }

        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate(IFormCollection Details)
        {
            if (await ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                Tuple<bool, string> Authentication = DataModels.User.Methods.Authenticate(Details["Email"], Details["Password"]);

                if (Authentication.Item1)
                {
                    await SignUserIn(Authentication.Item2);
                    AddNotification(new Notification("Welcome", "Welcome back " + Details["Email"], Enums.NotificationType.Info));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddNotification(new Notification("Error", Authentication.Item2, Enums.NotificationType.Error));
                    return RedirectToAction("Login");
                }
            }
            else
            {
                AddNotification(new Notification("Error", "Invalid Recaptcha", Enums.NotificationType.Error));
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            AddNotification(new Notification("Logged Out", "You have been logged out", Enums.NotificationType.Info));
            await SignClientOut();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> RecoverPassword(IFormCollection Details)
        {
            if (await ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                DataModels.User user = new DataModels.User();
                user.Email = Details["Email"].ToString();
                if (user.Exists())
                {
                    PasswordRecovery Recovery = new PasswordRecovery();
                    Recovery.UserID = DataModels.User.Methods.GetUserIDFromEmail(Details["Email"].ToString());
                    Recovery.Save();

                    Emails.TemplateBuilder Template = new Emails.TemplateBuilder(Enums.EmailTemplate.PasswordRecovery);
                    Template.ReplacePlaceholder("{RecoveryLink}", Configuration.configuration["SiteInfo:URL"].ToString() + "/Account/ResetPassword?RecoveryID=" + Recovery.Key);
                    Azure.Communication.SendEmail("Password Recovery", Template.Body.ToString(), new List<string>() { Details["Email"].ToString() });

                    AddNotification(new Notification("Recovery Sent", "Check your email for a link to reset your password.", Enums.NotificationType.Info));

                    return RedirectToAction("ForgotPassword");
                }
                else
                {
                    return RedirectToAction("ForgotPassword");
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword");
            }
        }

        public IActionResult ResetPassword(string RecoveryID)
        {
            PasswordRecovery PasswordRecovery = new PasswordRecovery(RecoveryID);
            if (DateTime.UtcNow < PasswordRecovery.Expiry)
            {
                ViewBag.RecoveryID = RecoveryID;
                return View("Views/Account/ResetPassword.cshtml");
            }
            else
            {
                AddNotification(new Notification("Recovery Link Expired", "The password recovery which you are trying to action has expired, please retry.", Enums.NotificationType.Error));
                return RedirectToAction("ForgotPassword");
            }

        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult SetNewPassword(IFormCollection Details, string RecoveryID)
        {
            ViewBag.RecoveryID = HttpUtility.UrlDecode(RecoveryID);
            if (Details["Password"].ToString() == Details["ConfirmPassword"])
            {
                PasswordRecovery Recovery = new PasswordRecovery(RecoveryID);
                DataModels.User User = new DataModels.User(Recovery.UserID);
                User.Salt = Security.CreateSalt();
                User.Password = Security.HashPassword(Details["Password"].ToString(), User.Salt);
                User.Save(true);

                AddNotification(new Notification("Password Reset", "Password reset, please log in using your new password.", Enums.NotificationType.Info));

                //Password recoveries are one time use
                Recovery.Delete();

                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "The passwords entered do not match";
                return View("Views/Account/ResetPassword.cshtml");
            }
        }
    }
}
