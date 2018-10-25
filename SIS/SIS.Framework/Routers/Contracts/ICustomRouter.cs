namespace SIS.Framework.Routers.Contracts
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.WebServer.API;

    public interface ICustomRouter : IHttpHandler
    {
        bool ContainsMapping(IHttpRequest httpRequest);
    }
}
