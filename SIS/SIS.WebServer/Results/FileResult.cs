namespace SIS.WebServer.Results
{
    using HTTP.Headers;
    using HTTP.Responses;

    public class FileResult : HttpResponse
    {
        public FileResult(byte[] content)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Content = content;
        }
        
    }
}
