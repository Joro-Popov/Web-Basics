namespace IRunes.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts.Base;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Services.Contracts;
    
    public class HomeController : BaseController
    {
        private readonly IUserService userService;

        public HomeController(IUserService userService) : base(userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!this.userService.IsAuthenticated(this.Request)) return this.View();
            
            var username = this.Request.Session.GetParameter("username").ToString();
            
            return this.RedirectToAction($"/Welcome?username={username}");
        }

        [HttpGet]
        public IActionResult Welcome(string username)
        {
            this.Model.Data["username"] = username;
            return this.View();
        }
    }
}
