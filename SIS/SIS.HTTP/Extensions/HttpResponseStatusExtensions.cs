namespace SIS.HTTP.Extensions
{
    using System;

    using Enums;

    public static class HttpResponseStatusExtensions
    {
        private const string StatusNotSupported = "Status code {0} not supported!";

        public static string GetResponseLine(this HttpResponseStatusCode statusCode)
        {
            string status;

            switch ((int)statusCode)
            {
                case 200: status = "OK"; break;
                case 201: status = "Created"; break;
                case 302: status = "Found"; break;
                case 303: status = "See Other"; break;
                case 400: status = "Bad Request"; break;
                case 401: status = "Unauthorized"; break;
                case 403: status = "Forbidden"; break;
                case 404: status = "Not Found"; break;
                case 500: status = "Internal Server Error"; break;
                default: throw new NotSupportedException(string.Format(StatusNotSupported, statusCode));
            }
            
            return $"{(int)statusCode} {status}";
        }

    }
}