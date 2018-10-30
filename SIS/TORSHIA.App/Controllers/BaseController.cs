namespace TORSHIA.App.Controllers
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Controllers;
    using Data;

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
                this.Model.Data["IsLogged"] = "block";

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
                this.Model.Data["IsLogged"] = "none";
                this.Model.Data["IsGuest"] = "block";
            }

            return base.View(viewName);
        }
    }
}
