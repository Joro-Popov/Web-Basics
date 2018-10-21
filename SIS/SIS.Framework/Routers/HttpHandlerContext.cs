namespace SIS.Framework.Routers
{
    using System.IO;
    using System.Linq;

    using HTTP.Common;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using WebServer.API;

    public class HttpHandlerContext : IHttpHandler
    {
        private readonly IHttpHandler controllerHandler;
        private readonly IHttpHandler resourceHandler;

        public HttpHandlerContext(IHttpHandler controllerHandler, IHttpHandler resourceHandler)
        {
            this.controllerHandler = controllerHandler;
            this.resourceHandler = resourceHandler;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            return this.IsResourceRequest(request) ? this.resourceHandler.Handle(request) : this.controllerHandler.Handle(request);
        }

        private bool IsResourceRequest(IHttpRequest httpRequest)
        {
            if (string.IsNullOrWhiteSpace(httpRequest.Path.Split('/').Last())) return false;

            var extension = Path.GetExtension(httpRequest.Path);

            return !string.IsNullOrWhiteSpace(extension) && GlobalConstants.FileExtensions.Contains(extension.Substring(1));
        }
    }
}
