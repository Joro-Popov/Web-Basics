namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class CustomRouter : ICustomRouter
    {
        public CustomRouter()
        {
            this.Routes = new Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>>
            {
                [HttpRequestMethod.Get] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Post] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Put] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>(),
                [HttpRequestMethod.Delete] = new Dictionary<string, Func<IHttpRequest, IHttpResponse>>()
            };
        }

        private Dictionary<HttpRequestMethod, Dictionary<string, Func<IHttpRequest, IHttpResponse>>> Routes { get; }

        public bool ContainsMapping(IHttpRequest httpRequest)
            => this.Routes.ContainsKey(httpRequest.RequestMethod)
               && this.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path.ToLower());

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            return this.ContainsMapping(httpRequest) ?
                this.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest) : null;
        }
    }
}
