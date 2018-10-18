namespace SIS.Framework.Controllers
{
    using System.Runtime.CompilerServices;
    using System.Text;

    using ActionResults;
    using ActionResults.Contracts;
    using Utilities;
    using Views;
    using Models;

    using HTTP.Requests.Contracts;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;

    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
            this.Response = new HttpResponse(HttpResponseStatusCode.Ok);
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public Model ModelState { get; } = new Model();
        
        protected ViewModel Model { get; }

        protected IViewable View(bool isLogged = true, [CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, viewName);

            var view = new View(fullyQualifiedName, this.Model.Data, isLogged);

            return new ViewResult(view);
        }

        protected  IRedirectable RedirectToAction(string redirectUrl) => new RedirectResult(redirectUrl);


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
