using SIS.Framework.Models;

namespace IRunes.Models.ViewModels.User
{
    public class RegisterViewModel
    {
        // TODO: Put validation attributes

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
    }
}
