namespace SIS.WebServer.Results
{
    using System.Text;

    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class BadRequestResult : HttpResponse
    {
        private const string ErrorMessage = "<h1>Error of type {0} occured!\r\n{1}</h1>";

        public BadRequestResult(string content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            //content = string.Format(ErrorMessage, (int)responseStatusCode, content);

            Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
