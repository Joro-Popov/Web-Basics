namespace IRunes.App.Controllers
{
    using Data;
    using SIS.Framework.Controllers;
    using SIS.Framework.Services.Contracts;

    public abstract class BaseController : Controller
    {
        protected BaseController(IUserService userService)
        {
            this.DbContext = new IRunesDbContext();
            this.UserService = userService;
        }

        protected IRunesDbContext DbContext { get; set; }

        protected IUserService UserService { get; set; }
    }
}
