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
    }
}
