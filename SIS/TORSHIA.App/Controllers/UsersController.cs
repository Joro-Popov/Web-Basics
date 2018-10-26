namespace TORSHIA.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Security;
    using SIS.Framework.Services.Contracts;
    using SIS.HTTP.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using SIS.Framework.Attributes.Action;
    using ViewModels.Users;
    using Models;

    public class UsersController : BaseController
    {
        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/logged");
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/logged");
            }

            if (!ModelState.IsValid.HasValue)
            {
                return this.View();
            }

            var hashedPassword = this.hashService.Hash(model.Password);

            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == hashedPassword);
            
            if (user == null) return this.View();

            var identityUser = new IdentityUser()
            {
                Username = user.Username,
                Password = hashedPassword,
                Email = user.Email,
                Roles = new List<string>() { user.Role.ToString() }
            };

            this.SignIn(identityUser);

            return this.RedirectToAction("/Home/Logged");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/logged");
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/logged");
            }

            if (!ModelState.IsValid.HasValue) return this.View();

            var email = WebUtility.UrlDecode(model.Email);

            var usernameIsInvalid = string.IsNullOrWhiteSpace(model.Username) || model.Username.Length < 3;
            var usernameExists = this.DbContext.Users.Any(u => u.Username == model.Username);
            var passwordIsNullOrEmpty = string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6;
            var passwordsMismatch = model.Password != model.ConfirmPassword;

            var emailIsInvalid = (string.IsNullOrWhiteSpace(email) || !email.Contains('@'));

            if (usernameIsInvalid || usernameExists ||
                passwordIsNullOrEmpty || passwordsMismatch || emailIsInvalid) return this.View();

            var hashedPassword = this.hashService.Hash(model.Password);

            var role = this.DbContext.Users.Any() ? UserRole.Admin : UserRole.User;

            var user = new User
            {
                Username = model.Username,
                Password = hashedPassword,
                Email = email,
                Role = role
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

            var identityUser = new IdentityUser()
            {
                Username = user.Username,
                Password = hashedPassword,
                Email = user.Email,
                Roles = new List<string>() { user.Role.ToString() }
            };

            this.SignIn(identityUser);

            return this.RedirectToAction("/home/logged");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            this.SignOut();

            return RedirectToAction("/home/index");
        }
    }
}
