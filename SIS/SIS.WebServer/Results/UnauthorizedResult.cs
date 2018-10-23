namespace SIS.WebServer.Results
{
    using System.Text;

    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class UnauthorizedResult : HttpResponse
    {
        private const string DEFAULT_ERROR_HEADING = 
            "<h1>You have no permissions to access this functionality,</h1>";

        public UnauthorizedResult() : base(HttpResponseStatusCode.Unauthorized)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html"));
            this.Content = Encoding.UTF8.GetBytes(DEFAULT_ERROR_HEADING);
        }
    }
}
