using System.Collections.Generic;
using TORSHIA.Models.Enums;

namespace TORSHIA.App.Controllers
{
    using ViewModels.Tasks;
    using SIS.Framework.ActionResults.Contracts;
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
                    }).ToList();

                var taskRowViewModels = new List<TasksRowViewModel>();

                for (int i = 0; i < tasks.Count(); i++)
                {
                    if (i % 5 == 0)
                    {
                        taskRowViewModels.Add(new TasksRowViewModel());
                    }

                    taskRowViewModels[taskRowViewModels.Count - 1].Tasks.Add(tasks[i]);
                }

                this.Model.Data["Username"] = this.Identity.Username;
                this.Model.Data["Tasks"] = taskRowViewModels;

                if (this.Identity.Roles.Contains(nameof(UserRole.Admin)))
                {
                    return this.View("Index-Admin");
                }
                else if (this.Identity.Roles.Contains(nameof(UserRole.User)))
                {
                    return this.View("Index-User");
                }

                return this.View();
            }
            
            return this.View();
        }
    }
}
