namespace SIS.WebServer.API
{
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    
    public interface IHttpHandler
    {
        IHttpResponse Handle(IHttpRequest request);
    }
}
