using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Attributes.Action;
using SIS.Framework.Attributes.Methods;
using SIS.HTTP.Exceptions;
using TORSHIA.App.ViewModels.Tasks;
using TORSHIA.Models;
using TORSHIA.Models.Enums;

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
                Description = WebUtility.UrlDecode(task.Description),
                AffectedSectors = string.Join(", ", task.AffectedSectors.Select(s => s.Sector.ToString()).ToList()),
                Level = task.AffectedSectors.Count,
                DueDate = task.DueDate.ToString("dd/MM/yyyy"),
                Participants = string.Join(", ", task.Participants.Select(p => p.User.Username).ToList())
            };

            this.Model.Data["Task"] = viewModel;

            return this.View();
        }

        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize("Admin")]
        public IActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid.HasValue) return this.View();

            if (this.DbContext.Tasks.Any(t => t.Title == model.Title))
            {
                return this.RedirectToAction("/tasks/create");
            }

            var task = new Task()
            {
                Title = WebUtility.UrlDecode(model.Title),
                Description = model.Description,
                DueDate = DateTime.ParseExact(model.DueDate, "yyyy-MM-dd",null),
                Level = model.AffectedSectors.Count,
            };

            var affectedSectors = model.AffectedSectors
                .Select(s => new TaskSector()
                {
                    Task = task,
                    Sector = (Sector) Enum.Parse(typeof(Sector), s, true)
                }).ToList();

            if (!string.IsNullOrWhiteSpace(model.Participants))
            {
                var participanstsAsStrings = WebUtility.UrlDecode(model.Participants)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var p in participanstsAsStrings)
                {
                    if (!this.DbContext.Users.Any(u => u.Username == p))
                    {
                        return this.RedirectToAction("/tasks/create");
                    }
                }

                var participants = participanstsAsStrings
                    .Select(p => new UserTask()
                    {
                        UserId = this.DbContext.Users.FirstOrDefault(u => u.Username == p).Id,
                        Task = task
                    }).ToList();

                task.Participants = participants;
            }

            task.AffectedSectors = affectedSectors;

            try
            {
                this.DbContext.Tasks.Add(task);
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.Message);
            }

            return this.RedirectToAction("/");
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Report(int TaskId)
        {
            if (!this.DbContext.Tasks.Any(t => t.Id == TaskId))
            {
                return this.RedirectToAction("/");
            }
            
            var user = this.DbContext.Users.FirstOrDefault(u => u.Username == this.Identity.Username);
            var task = this.DbContext.UserTasks.FirstOrDefault(ut => ut.TaskId == TaskId && ut.UserId == user.Id);

            if (this.DbContext.Reports.Any(r => r.ReporterId == user.Id && r.TaskId == TaskId))
            {
                return this.RedirectToAction("/");
            }

            
            task.Task.IsReported = true;

            this.DbContext.UserTasks.Remove(task);

            var rnd = new Random();
            var chance = rnd.Next(1,2);
            
            var report = new Report()
            {
                TaskId = TaskId,
                ReporterId = user.Id,
                ReportedOn = DateTime.Now,
                Status = (ReportStatus)chance
            };

            try
            {
                this.DbContext.Reports.Add(report);
                this.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InternalServerException(e.InnerException.Message);
            }

            return RedirectToAction("/");
        }
    }
}
