using System.Text;
using SIS.Framework.Services.Contracts;
using SIS.HTTP.Enums;
using SIS.HTTP.Headers;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;

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
            this.Response = new HttpResponse(HttpResponseStatusCode.Ok);
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

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
        
        public IHttpResponse ServerErrorResult(string content)
        {
            content = string.Format(content, (int)HttpResponseStatusCode.InternalServerError, content);

            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);

            return this.Response;
        }
    }
}
