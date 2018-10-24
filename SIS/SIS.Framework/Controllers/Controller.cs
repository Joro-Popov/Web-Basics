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
                viewContent = this.ViewEngine.GetViewContent(this.Identity != null, controllerName, viewName);
            }
            catch (Exception e)
            {
                this.Model.Data["Error"] = e.Message;
                
                viewContent = this.ViewEngine.GetErrorContent(this.Identity != null);
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
    }
}
