namespace SIS.WebServer.Results
{
    using System.Text;

    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class ServerErrorResult : HttpResponse
    {
        private const string ErrorMessage = "<h1>Error {0}</h1>";

        public ServerErrorResult(string content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            content = string.Format(ErrorMessage, (int)responseStatusCode, content);

            Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
