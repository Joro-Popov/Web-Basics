using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Methods;

namespace TORSHIA.App.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
