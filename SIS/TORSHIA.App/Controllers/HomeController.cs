using System;
using TORSHIA.App.ViewModels.Tasks;

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

            Console.WriteLine(DateTime.Now);
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

            var tasks = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username)
                .UserTasks
                .Select(t => t.Task)
                .ToList()
                .Select(t => new TaskViewModel()
                {
                    Title = t.Title,
                    Level = t.AffectedSectors.Count
                });

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

            this.Model.Data["Tasks"] = tasks;

            return this.View();
        }
    }
}
