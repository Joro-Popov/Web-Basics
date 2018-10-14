namespace SIS.HTTP.Responses
{
    using System;
    using System.Linq;
    using System.Text;

    using Common;
    using Enums;
    using Extensions;
    using Headers;
    using Contracts;
    using Cookies;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Cookies.Contracts;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
        }

        public HttpResponse(HttpResponseStatusCode responseStatusCode)
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];
            this.StatusCode = responseStatusCode;
        }
        
        public HttpResponseStatusCode StatusCode { get; set; }
        public IHttpHeaderCollection Headers { get; }
        public IHttpCookieCollection Cookies { get; }
        public byte[] Content { get; set; }

        public void AddCookie(HttpCookie cookie)
        {
            this.Cookies.Add(cookie);
        }

        public void AddHeader(HttpHeader header)
        {
            this.Headers.Add(header);
        }

        public byte[] GetBytes()
        {
            var responseBytes = Encoding.UTF8.GetBytes(this.ToString())
                .Concat(this.Content).ToArray();

            return responseBytes;
        }

        public override string ToString()
        {
            var response = new StringBuilder();

            response.AppendLine($"{GlobalConstants.HttpProtocolFragment} {this.StatusCode. GetResponseLine()}")
                    .AppendLine(string.Join(Environment.NewLine, this.Headers));

            if (this.Cookies.HasCookies())
            {
                foreach (var cookie in this.Cookies)
                {
                    response.AppendLine($"{HttpHeader.SetCookie}: {cookie}");
                }
            }

            response.AppendLine();

            return response.ToString();
        }
    }
}