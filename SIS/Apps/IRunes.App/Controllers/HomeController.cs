namespace IRunes.App.Controllers
{
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.ActionResults.Contracts;

    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (this.Identity != null)
            {
                return this.RedirectToAction($"/home/welcome");
            }

            return this.View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Welcome()
        {
            this.Model.Data["username"] = this.Identity.Username;

            return this.View();
        }
    }
}
