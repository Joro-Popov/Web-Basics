namespace SIS.HTTP.Responses.Contracts
{
    using Enums;
    using Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Cookies.Contracts;
    using Cookies;

    public interface IHttpResponse
    {
        HttpResponseStatusCode StatusCode { get; set; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        byte[] Content { get; set; }

        void AddHeader(HttpHeader header);

        void AddCookie(HttpCookie cookie);

        byte[] GetBytes();
    }
}