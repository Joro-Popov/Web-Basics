namespace IRunes.App.Controllers
{
    using Data;
    using SIS.Framework.Controllers;
    using SIS.Framework.Services.Contracts;

    public abstract class BaseController : Controller
    {
        protected BaseController(IAuthenticationService authenticationService)
        {
            this.DbContext = new IRunesDbContext();
            this.AuthenticationService = authenticationService;
        }

        protected IRunesDbContext DbContext { get; set; }

        protected IAuthenticationService AuthenticationService { get; set; }
    }
}
