namespace SIS.HTTP.Requests.Contracts
{
    using System.Collections.Generic;

    using Enums;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Sessions.Contracts;
    using SIS.HTTP.Cookies.Contracts;

    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        Dictionary<string, object> FormData { get; }

        Dictionary<string, object> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        HttpRequestMethod RequestMethod { get; }

        IHttpCookieCollection Cookies { get; }

        IHttpSession Session { get; set; }
    }
}