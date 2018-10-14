namespace IRunes.App.Controllers
{
    using System.IO;
    using SIS.HTTP.Enums;
    using Data;
    using SIS.HTTP.Responses.Contracts;
    using System.Runtime.CompilerServices;
    using SIS.WebServer.Results;
    using System.Collections.Generic;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.Framework.Controllers;

    public abstract class BaseController : Controller
    {
        private const string RootDirectory = "../../../Views";
        private const string LayoutDirectory = "../../../Views/_Layout.html";
        private const string FileExtension = ".html";
        private const string MissingFile = "View {0} not found";
        
        protected BaseController()
        {
            this.DbContext = new IRunesDbContext();
            this.CookieService = new CookieService();
            this.ViewBag = new Dictionary<string, string>();
        }

        protected IRunesDbContext DbContext { get; set; }

        protected IDictionary<string, string> ViewBag { get; set; }

        protected ICookieService CookieService { get; set; }

        //protected IHttpResponse View([CallerMemberName] string viewName = "")
        //{
        //    var viewPath = $"{RootDirectory}/{this.GetControllerName()}/{viewName}{FileExtension}";
            
        //    if (!File.Exists(viewPath)) return new BadRequestResult(string.Format(MissingFile, viewName), HttpResponseStatusCode.NotFound);

        //    var content = File.ReadAllText(viewPath);
        //    var layout = File.ReadAllText(LayoutDirectory);
            
        //    foreach (var key in this.ViewBag.Keys)
        //    {
        //        if (content.Contains($"{{{{{key}}}}}"))
        //        {
        //            content = content.Replace($"{{{{{key}}}}}", this.ViewBag[key]);
        //        }
        //    }

        //    layout = layout.Replace("@RenderBody", content);

        //    return new HtmlResult(layout, HttpResponseStatusCode.Ok);
        //}

        protected void SignInUser(string username, IHttpRequest request, IHttpResponse response)
        {
            var cookieContent = this.CookieService.SetUserCookie(username);
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
