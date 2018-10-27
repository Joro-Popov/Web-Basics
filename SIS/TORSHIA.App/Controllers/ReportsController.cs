namespace TORSHIA.App.Controllers
{
    using System.Globalization;
    using System.Linq;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using ViewModels.Reports;

    public class ReportsController : BaseController
    {
        [HttpGet]
        [Authorize("Admin")]
        public IActionResult All()
        {
            var reports = this.DbContext.Reports.ToList()
                .Select((r,i) => new ReportViewModel()
                {
                    Index = i + 1,
                    ReportId = r.Id,
                    TaskTitle = r.Task.Title,
                    Status = r.Status.ToString(),
                    TaskLevel = r.Task.Level
                }).ToList();

            this.Model.Data["Reports"] = reports;

            return this.View();
        }

        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Details(int reportId)
        {
            var report = this.DbContext.Reports.FirstOrDefault(r => r.Id == reportId);

            var details = new ReportDetailsViewModel()
            {
                ReportId = report.Id,
                TaskTitle = report.Task.Title,
                TaskLevel = report.Task.Level,
                TaskDescription = report.Task.Description,
                Status = report.Status.ToString(),
                Reporter = report.Reporter.Username,
                ReportedOn = report.ReportedOn.ToString(CultureInfo.InvariantCulture),
                DueDate = report.Task.DueDate.ToString(CultureInfo.InvariantCulture),
                Participants = string.Join(',',report.Task.Participants.Select(p => p.User.Username)),
                AffectedSectors = string.Join(',', report.Task.AffectedSectors.Select(s => s.Sector.ToString()))
            };

            this.Model.Data["Details"] = details;

            return this.View();
        }
    }
}
