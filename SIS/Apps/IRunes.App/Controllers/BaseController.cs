namespace IRunes.App.Controllers
{
    using Data;
    using SIS.Framework.Controllers;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.DbContext = new IRunesDbContext();
        }

        protected IRunesDbContext DbContext { get; set; }
    }
}
