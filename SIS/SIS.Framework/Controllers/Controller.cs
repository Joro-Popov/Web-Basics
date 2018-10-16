namespace SIS.Framework.Controllers
{
    using System.Runtime.CompilerServices;

    using ActionResults;
    using ActionResults.Contracts;
    using Utilities;
    using Views;
    using Models;
    using HTTP.Requests.Contracts;

    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
        }

        public IHttpRequest Request { get; set; }
        
        public Model ModelState { get; } = new Model();

        protected ViewModel Model { get; }

        protected IViewable View([CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, viewName);

            var view = new View(fullyQualifiedName, this.Model.Data);

            return new ViewResult(view);
        }

        protected  IRedirectable RedirectToAction(string redirectUrl) => new RedirectResult(redirectUrl);
    }
}
