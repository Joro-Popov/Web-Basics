﻿namespace SIS.Framework.Routers
{
    using System.IO;
    using System.Linq;

    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using Contracts;
    using WebServer.Results;

    public class ResourceRouter : IResourceRouter
    {
        private const string RootDirectoryRelativePath = "../../../";

        private const string ResourceFolderPath = "Resources/";

        private static readonly string[] AllowedResourceExtensions = { ".js", ".css", ".ico", ".jpg", ".jpeg", ".png", ".gif", ".html" };

        private string FormatResourcePath(string httpRequestPath)
        {
            var indexOfStartOfExtension = httpRequestPath.LastIndexOf('.');
            var indexOfStartOfNameOfResource = httpRequestPath.LastIndexOf('/');

            var requestPathExtension = httpRequestPath
                .Substring(indexOfStartOfExtension);

            var resourceName = httpRequestPath
                .Substring(
                    indexOfStartOfNameOfResource);

            return RootDirectoryRelativePath
                   + ResourceFolderPath
                   + requestPathExtension.Substring(1)
                   + resourceName;
        }

        private bool IsAllowedExtension(string httpRequestPath)
        {
            var requestPathExtension = httpRequestPath
                .Substring(httpRequestPath.LastIndexOf('.'));

            return AllowedResourceExtensions.Contains(requestPathExtension);
        }

        public bool IsResourceRequest(string httpRequestPath) => httpRequestPath.Contains('.');

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            if (this.IsAllowedExtension(httpRequest.Path))
            {
                string httpRequestPath = httpRequest.Path;

                string resourcePath = this.FormatResourcePath(httpRequestPath);

                if (!File.Exists(resourcePath))
                {
                    return null;
                }

                var fileContent = File.ReadAllBytes(resourcePath);

                return new InlineResourceResult(fileContent, HttpResponseStatusCode.Ok);
            }

            return null;
        }
    }
}
