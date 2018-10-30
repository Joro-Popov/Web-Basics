﻿namespace TORSHIA.App.Controllers
{
    using ViewModels.Tasks;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using System.Linq;

    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (this.Identity != null)
            {
                var tasks = this.DbContext.Users
                    .FirstOrDefault(u => u.Username == this.Identity.Username)?
                    .UserTasks
                    .Select(t => t.Task)
                    .ToList()
                    .Select(t => new TaskViewModel()
                    {
                        TaskId = t.Id,
                        Title = t.Title,
                        Level = t.AffectedSectors.Count
                    });

                if (this.Identity == null)
                {
                    return this.RedirectToAction("/home/index");
                }

                this.Model.Data["Username"] = this.Identity.Username;
                this.Model.Data["Tasks"] = tasks;

                return this.View();
            }
            
            return this.View();
        }
    }
}
