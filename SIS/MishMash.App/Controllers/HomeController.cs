using SIS.Framework.ActionResults.Contracts.Base;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;

namespace MishMash.App.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        [Authorize("User")]
        public IActionResult Authorized()
        {
            this.Model.Data["Username"] = this.Identity.Username;

            return this.View();
        }
    }
}
