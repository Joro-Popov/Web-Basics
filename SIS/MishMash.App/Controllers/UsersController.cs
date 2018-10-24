﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MishMash.Models;
using MishMash.Models.Enums;
using MishMash.Models.ViewModels.Users;
using SIS.Framework.ActionResults.Contracts.Base;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;
using SIS.Framework.Security;
using SIS.Framework.Services.Contracts;
using SIS.HTTP.Exceptions;

namespace MishMash.App.Controllers
{
    public class UsersController : BaseController
    {
        private IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/authorized");
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
            
            var identityUser = new IdentityUser()
            {
                Username = user.Username,
                Password = hashedPassword,
                Email = user.Email,
                Roles = new List<string>() { user.Role.ToString()}
            };

            this.SignIn(identityUser);

            return this.RedirectToAction("/Home/Authorized");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/authorized");
            }
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
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

            var user = new User
            {
                Username = model.Username,
                Password = hashedPassword,
                Email = email,
                Role = Role.User
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
                Roles = new List<string>() { user.Role.ToString()}
            };

            this.SignIn(identityUser);

            return this.RedirectToAction("/home/authorized");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            this.SignOut();

            return this.RedirectToAction("/home/index");
        }
    }
}