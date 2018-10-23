using SIS.Framework.Attributes.Action;

namespace IRunes.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Methods;
    
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
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
