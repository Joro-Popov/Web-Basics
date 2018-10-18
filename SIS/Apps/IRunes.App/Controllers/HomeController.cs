namespace IRunes.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Services.Contracts;
    
    public class HomeController : BaseController
    {
        private readonly IAuthenticationService authenticationService;

        public HomeController(IAuthenticationService authenticationService) : base(authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!this.authenticationService.IsAuthenticated(this.Request)) return this.View(false);
            
            var username = this.Request.Session.GetParameter("username").ToString();
            
            return this.RedirectToAction($"/Home/Welcome?username={username}");
        }

        [HttpGet]
        public IActionResult Welcome(string username)
        {
            this.Model.Data["username"] = username;

            return this.View();
        }
    }
}
