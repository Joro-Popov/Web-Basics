namespace TORSHIA.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using System.Linq;
    using TORSHIA.App.ViewModels.Home;

    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction("/home/logged");
            }
            return this.View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logged()
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/home/index");
            }

            if (this.Identity.Roles.Contains("Admin"))
            {
                this.Model.Data["Username"] = new AdminViewModel() { Username = $"Admin-{this.Identity.Username}" };
            }
            else
            {
                this.Model.Data["Username"] = new UserViewModel() { Username = this.Identity.Username };
            }
            
            return this.View();
        }
    }
}
