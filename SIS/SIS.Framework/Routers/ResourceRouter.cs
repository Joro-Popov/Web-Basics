namespace SIS.Framework.Routers
{
    using System.IO;
    using System.Linq;
    using System.Text;

    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using WebServer.API;
    using WebServer.Results;

    public class ResourceRouter : IHttpHandler
    {
        private const string RELATIVE_PATH = "../../../Resources";

        public IHttpResponse Handle(IHttpRequest request)
        {
            return GetResource(request.Path);
        }

        private IHttpResponse GetResource(string path)
        {
            var file = path.Split('/').Last();

            var fileExtension = Path.GetExtension(path).Substring(1);

            var resourcePath = $"{RELATIVE_PATH}/{fileExtension}/{file}";

            var resource = File.ReadAllText(resourcePath);

            var resourceBytes = Encoding.UTF8.GetBytes(resource);

            return new InlineResourceResult(resourceBytes, HttpResponseStatusCode.Ok);
        }
    }
}
