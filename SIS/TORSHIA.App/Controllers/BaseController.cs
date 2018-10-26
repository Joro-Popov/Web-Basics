using System.Linq;
using System.Runtime.CompilerServices;
using SIS.Framework.ActionResults.Contracts;
using SIS.Framework.Controllers;
using TORSHIA.Data;

namespace TORSHIA.App.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.DbContext = new TorshiaDbContext();
        }

        protected TorshiaDbContext DbContext { get; set; }

        protected override IViewable View([CallerMemberName] string viewName = "")
        {
            if (this.Identity != null)
            {
                if (this.Identity.Roles.Contains("Admin"))
                {
                    this.Model.Data["IsAdmin"] = "block";
                    this.Model.Data["IsUser"] = "none";
                    this.Model.Data["IsGuest"] = "none";
                }
                else
                {
                    this.Model.Data["IsAdmin"] = "none";
                    this.Model.Data["IsUser"] = "block";
                    this.Model.Data["IsGuest"] = "none";
                }
            }
            else
            {
                this.Model.Data["IsAdmin"] = "none";
                this.Model.Data["IsUser"] = "none";
                this.Model.Data["IsGuest"] = "block";
            }
            return base.View(viewName);
        }
    }
}
