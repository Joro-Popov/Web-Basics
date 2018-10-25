using System.Linq;
using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;
using TORSHIA.App.ViewModels.Tasks;

namespace TORSHIA.App.Controllers
{
    public class TasksController : BaseController
    {
        [HttpGet]
        [Authorize]
        public IActionResult Details(int Id)
        {
            if (this.Identity == null)
            {
                return this.RedirectToAction("/home/index");
            }

            var user = this.DbContext.Users
                .FirstOrDefault(u => u.Username == this.Identity.Username);

            var task = user.UserTasks
                .FirstOrDefault(t => t.TaskId == Id && t.UserId == user.Id)
                .Task;

            var viewModel = new DetailsViewModel()
            {
                Title = task.Title,
                Description = task.Description,
                AffectedSectors = string.Join(", ", task.AffectedSectors.Select(s => s.Sector.ToString()).ToList()),
                Level = task.AffectedSectors.Count,
                DueDate = task.DueDate.ToString("dd/MM/yyyy"),
                Participants = string.Join(", ", task.Participants.Select(p => p.User.Username).ToList())
            };

            this.Model.Data["Task"] = viewModel;

            return this.View();
        }
    }
}
