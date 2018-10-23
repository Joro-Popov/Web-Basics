namespace SIS.Framework.Controllers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    using ActionResults;
    using ActionResults.Contracts;
    using Utilities;
    using Views;
    using Models;
    using Security.Contracts;

    using HTTP.Requests.Contracts;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;

    public abstract class Controller
    {
        private const string IDENTITY_KEY = "auth";

        protected Controller()
        {
            this.Model = new ViewModel();
            this.Response = new HttpResponse(HttpResponseStatusCode.Ok);
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public Model ModelState { get; } = new Model();
        
        protected ViewModel Model { get; }
        
        private ViewEngine ViewEngine { get; } = new ViewEngine();
        
        protected IViewable View([CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, viewName);
            }
            catch (Exception e)
            {
                this.Model.Data["Error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            var renderedContent = this.ViewEngine.RenderHtml(viewContent, this.Model.Data);

            return new ViewResult(new View(renderedContent));
            
        }

        protected  IRedirectable RedirectToAction(string redirectUrl) => new RedirectResult(redirectUrl);

        protected void SignIn(IIdentity auth)
        {
            this.Request.Session.AddParameter(IDENTITY_KEY, auth);
        }

        protected void SignOut()
        {
            //var cookie = this.Request.Cookies.GetCookie(".auth");

            //cookie.Delete();

            //this.Response.Cookies.Add(cookie);

            this.Request.Session.ClearParameters();
        }

        public IIdentity Identity
        {
            get
            {
                if (this.Request.Session.ContainsParameter(IDENTITY_KEY))
                {
                   return (IIdentity)this.Request.Session.GetParameter(IDENTITY_KEY);
                }

                return null; 
            }
        }

        public IHttpResponse HtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);

            return this.Response;
        }

        public IHttpResponse RedirectResult(string location)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.Location, location));
            this.Response.StatusCode = HttpResponseStatusCode.SeeOther;

            return this.Response;
        }

        public IHttpResponse FileResult(byte[] content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Response.Content = content;

            return this.Response;
        }
    }
}
