namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ActionResults.Contracts;
    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;
    using WebServer.API;
    using WebServer.Results;

    public class ServerRoutingTable : IHttpHandler
    {
        private const string NotSupportedViewResult = "The view result is not supported!";

        public ServerRoutingTable()
        {
            Routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IActionResult>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IActionResult>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IActionResult>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IActionResult>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IActionResult>>()
            };
        }

        public Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IActionResult>>> Routes { get; set; }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var isResourceRequest = this.IsResourceRequest(request);

            var requestMethodDoesNotExists = !this.Routes.ContainsKey(request.RequestMethod);
            var pathDoesNotExists = !this.Routes[request.RequestMethod].ContainsKey(request.Path);

            if (requestMethodDoesNotExists || pathDoesNotExists)
            {
                return isResourceRequest ? this.ReturnResource(request.Path) : new HttpResponse(HttpResponseStatusCode.NotFound);
            }
            
            var action = this.Routes[request.RequestMethod][request.Path].Invoke(request);
            var result = action.Invoke();

            switch (action)
            {
                case IViewable _: return new HtmlResult(result, HttpResponseStatusCode.Ok);

                case IRedirectable _: return new RedirectResult(result);

                default: throw new InvalidOperationException(NotSupportedViewResult);
            }
        }

        private IHttpResponse ReturnResource(string path)
        {
            var file = path.Split('/').Last();

            var fileExtension = Path.GetExtension(path).Substring(1);

            var resourcePath = $"../../../Resources/{fileExtension}/{file}";

            var resource = File.ReadAllText(resourcePath);

            var resourceBytes = Encoding.UTF8.GetBytes(resource);

            return new InlineResourceResult(resourceBytes, HttpResponseStatusCode.Ok);
        }

        private bool IsResourceRequest(IHttpRequest httpRequest)
        {
            if (string.IsNullOrWhiteSpace(httpRequest.Path.Split('/').Last())) return false;

            var extension = Path.GetExtension(httpRequest.Path);

            return !string.IsNullOrWhiteSpace(extension) && GlobalConstants.FileExtensions.Contains(extension.Substring(1));
        }
    }
}