using SIS.WebServer.API;

namespace SIS.Framework.Routers.Contracts
{
    public interface IResourceRouter : IHttpHandler
    {
        bool IsResourceRequest(string httpRequestPath);
    }
}
