using SIS.Framework.Services.Contracts;

namespace IRunes.App.Controllers
{
    using Data;
    using SIS.HTTP.Responses.Contracts;
    using System.Collections.Generic;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.Framework.Controllers;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            this.DbContext = new IRunesDbContext();
            this.ViewBag = new Dictionary<string, string>();
        }

        protected IRunesDbContext DbContext { get; set; }

        protected IDictionary<string, string> ViewBag { get; set; }

        protected void SignInUser(string username, IHttpRequest request, IHttpResponse response, ICookieService cookieService)
        {
            var cookieContent = cookieService.SetUserCookie(username);
            var cookie = new HttpCookie(".auth-IRunes", cookieContent, 7);

            response.Cookies.Add(cookie);
            request.Session.AddParameter("username", username);
        }

        protected bool IsAuthenticated(IHttpRequest request)
        {
            return request.Session.ContainsParameter("username") && request.Cookies.ContainsCookie(".auth-IRunes");
        }
        
        private string GetControllerName() => this.GetType().Name.Replace("Controller", string.Empty);
    }
}
