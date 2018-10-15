using SIS.Framework.Models;
using SIS.Framework.Services;
using SIS.Framework.Services.Contracts;

namespace IRunes.App.Controllers
{
    using System;
    using SIS.HTTP.Requests.Contracts;
    using System.Linq;
    using System.Net;
    using Models;
    using SIS.HTTP.Enums;
    using SIS.WebServer.Results;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Methods;

    public class UsersController : BaseController
    {
        private const int UsernameMinLength = 3;
        private const int PasswordMinLength = 6;
        
        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return this.IsAuthenticated(this.Request) ? this.View("/") : this.View();
        }
        
        [HttpPost]
        public IActionResult Login(ViewModel model)
        {
            //var username = this.Request.FormData["username"].ToString();
           // var password = this.Request.FormData["password"].ToString();

            var username = model.Data["username"].ToString();
            var password = model.Data["password"].ToString();

            var hashedPassword = this.hashService.Hash(password.ToString());

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user == null) return this.View("/Users/Login");
 
            var response = this.View("/");
            
            //this.SignInUser(username, request, response);
            
            return response;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return this.IsAuthenticated(this.Request) ? this.View("/") : this.View();
        }

        [HttpPost]
        public IActionResult RegisterPost()
        {
            var username = this.Request.FormData["username"].ToString();
            var password = this.Request.FormData["password"].ToString();
            var confirmPassword = this.Request.FormData["confirmPassword"].ToString();
            var email = WebUtility.UrlDecode(this.Request.FormData["email"].ToString());
            
            var usernameIsInvalid = string.IsNullOrWhiteSpace(username) || username.Length < UsernameMinLength;
            var usernameExists = this.DbContext.Users.Any(u => u.Username == username);
            var passwordIsNullOrEmpty = string.IsNullOrWhiteSpace(password) || password.Length < PasswordMinLength;
            var passwordsMismatch = password != confirmPassword;
            var emailIsInvalid = (string.IsNullOrWhiteSpace(email) || !email.Contains('@'));
            
            if(usernameIsInvalid || usernameExists ||
               passwordIsNullOrEmpty || passwordsMismatch || emailIsInvalid) return  this.View("/Users/Register");

            var hashedPassword = this.hashService.Hash(password);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            this.DbContext.Users.Add(user);

            try
            {
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                //return new ServerErrorResult(e.Message, HttpResponseStatusCode.InternalServerError);
            }

            var response = this.View("/");

            //this.SignInUser(username, request, response);

            return response;
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var cookie = this.Request.Cookies.GetCookie(".auth-IRunes");

            cookie.Delete();

            var response = this.View("/");

            //response.Cookies.Add(cookie);

            return response;
        }
    }
}
