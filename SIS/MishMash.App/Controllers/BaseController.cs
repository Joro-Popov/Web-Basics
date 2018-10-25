namespace MishMash.App.Controllers
{
    using MishMash.Data;
    using SIS.Framework.Controllers;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.DbContext = new MishMashDbContext();
        }

        protected MishMashDbContext DbContext { get; set; }
    }
}
