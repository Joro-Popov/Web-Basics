using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;

namespace TORSHIA.App.Controllers
{
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
            return this.View();
        }
    }
}
