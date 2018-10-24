namespace IRunes.App.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Services.Contracts;
    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.HTTP.Exceptions;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Security;

    using Models.Domain;
    using Models.ViewModels.User;
    
    public class UsersController : BaseController
    {
        private const int UsernameMinLength = 3;
        private const int PasswordMinLength = 6;
        
        private readonly IHashService hashService;

        public UsersController(IHashService hashService) : base()
        {
            this.hashService = hashService;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/welcome");
            }

            return this.View();
        }
        
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid.HasValue)
            {
                return this.View();
            }

            var hashedPassword = this.hashService.Hash(model.Password);

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == hashedPassword);

            if (user == null) return this.View();

            var result = this.RedirectToAction("/Home/Welcome");
            
            var identityUser = new IdentityUser()
            {
                Username = user.Username,
                Password = hashedPassword,
                Email = user.Email
            };

            this.SignIn(identityUser);
            
            return result;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/welcome");
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid.HasValue) return this.View();

            var email = WebUtility.UrlDecode(model.Email);
            
            var usernameIsInvalid = string.IsNullOrWhiteSpace(model.Username) || model.Username.Length < UsernameMinLength;
            var usernameExists = this.DbContext.Users.Any(u => u.Username == model.Username);
            var passwordIsNullOrEmpty = string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < PasswordMinLength;
            var passwordsMismatch = model.Password != model.ConfirmPassword;

            var emailIsInvalid = (string.IsNullOrWhiteSpace(email) || !email.Contains('@'));
            
            if(usernameIsInvalid || usernameExists ||
               passwordIsNullOrEmpty || passwordsMismatch || emailIsInvalid) return  this.View();

            var hashedPassword = this.hashService.Hash(model.Password);

            var user = new User
            {
                Username = model.Username,
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
                throw new InternalServerException(e.Message);
            }
            
            var response = this.RedirectToAction("/home/welcome");

            var identityUser = new IdentityUser()
            {
                Username = user.Username,
                Password = hashedPassword,
                Email = user.Email
            };

            this.SignIn(identityUser);
            
            return response;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            this.SignOut();

            var response = this.RedirectToAction("/home/index");
            
            return response;
        }
    }
}
