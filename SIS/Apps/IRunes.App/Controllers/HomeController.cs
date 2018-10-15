namespace IRunes.App.Controllers
{
    using SIS.Framework.ActionResults.Contracts;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (!this.IsAuthenticated(this.Request)) return this.View();
            
            var username = this.Request.Session.GetParameter("username").ToString();

            this.ViewBag["username"] = username;

            return this.View("Welcome");
        }
    }
}
