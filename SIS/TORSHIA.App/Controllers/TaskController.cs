using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;

namespace TORSHIA.App.Controllers
{
    public class TaskController : BaseController
    {
        [HttpGet]
        [Authorize]
        public IActionResult Details(int taskId)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/home/index");
            }

            return this.View();
        }
    }
}
