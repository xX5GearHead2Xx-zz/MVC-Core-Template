using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ecommerce.DataModels;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Net;
using Ecommerce.ViewModels;

namespace Ecommerce.Controllers
{
    public class BaseController : Controller
    {
        public string UserID
        {
            get
            {
                return User.Claims.Where(c => c.Type == "UserID").First().Value;
            }
        }

        public async Task<bool> SignUserIn(string UserID)
        {
            if (!string.IsNullOrEmpty(UserID))
            {
                DataModels.User User = new DataModels.User(UserID);

                List<Claim> ClientClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, User.Email),
                    new Claim("UserID", UserID)
                };

                foreach (LinkUserRole UserRole in User.Roles)
                {
                    string Role = UserRole.RoleID.ToString() + UserRole.AccessType.ToString();
                    ClientClaims.Add(new Claim(ClaimTypes.Role, Role));
                }

                ClaimsIdentity ClientIdentity = new ClaimsIdentity(ClientClaims, "Site Identity");

                ClaimsPrincipal ClientPrincipal = new ClaimsPrincipal(new[] { ClientIdentity });

                await HttpContext.SignInAsync(ClientPrincipal);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SignClientOut()
        {
            await HttpContext.SignOutAsync();
            return true;
        }

        public class RecaptchaObject
        {
            public bool success { get; set; } = false;
        }

        public async Task<bool> ValidateRecaptcha(string Captcha)
        {
            Uri RequestUri = new Uri("https://www.google.com/recaptcha/api/siteverify?secret=" + Configuration.configuration["Recaptcha:RecaptchaPrivate"] + "&response=" + Captcha);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, RequestUri);

            var client = new HttpClient();
            var response = client.Send(httpRequest);

            RecaptchaObject result = JsonConvert.DeserializeObject<RecaptchaObject>(await response.Content.ReadAsStringAsync());

            return result.success;
        }

        public void AddNotification(Notification Notification)
        {
            List<Notification> Notifications;
            if (TempData["Notifications"] != null)
            {
                Notifications = JsonConvert.DeserializeObject<List<Notification>>(TempData["Notifications"].ToString());
            }
            else
            {
                Notifications = new List<Notification>();
            }
            
            Notifications.Add(Notification);
            TempData["Notifications"] = JsonConvert.SerializeObject(Notifications);
        }
    }
}
