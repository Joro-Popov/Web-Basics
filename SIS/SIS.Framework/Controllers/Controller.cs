namespace SIS.Framework.Controllers
{
    using System;
    using System.Runtime.CompilerServices;

    using ActionResults;
    using ActionResults.Contracts;
    using Utilities;
    using Views;
    using Models;
    using Security.Contracts;

    using HTTP.Requests.Contracts;

    public abstract class Controller
    {
        private const string IDENTITY_KEY = "auth";
        
        public IHttpRequest Request { get; set; }
        
        public Model ModelState { get; } = new Model();
        
        protected ViewModel Model { get; } = new ViewModel();
        
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
