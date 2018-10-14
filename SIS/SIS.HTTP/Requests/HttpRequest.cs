namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Common;
    using Enums;
    using Exceptions;
    using Contracts;
    using Headers;
    using Cookies;
    using Extensions;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Sessions.Contracts;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpCookieCollection Cookies { get; }

        public IHttpSession Session { get; set; }


        private void ParseRequest(string requestString)
        {
            var requestContent = requestString.Split(Environment.NewLine);

            var requestLine = requestString
                .Split(Environment.NewLine)
                .First()
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var formData = requestContent[requestContent.Length - 1];

            if (!IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();
            this.ParseHeaders(requestContent.ToArray());
            this.ParseCookies();
            this.ParseRequestParameters(formData);
        }

        private void ParseCookies()
        {
            var cookieHeader = this.Headers.GetHeader(HttpHeader.Cookie);

            if(cookieHeader == null) return;

            var cookieString = cookieHeader.Value;

            if (string.IsNullOrEmpty(cookieString)) return;

            var splitCookies = cookieString.Split("; ",StringSplitOptions.RemoveEmptyEntries);

            foreach (var cookie in splitCookies)
            {
                var cookieParts = cookie.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

                if (cookieParts.Length != 2) continue;
                
                var cookieName = cookieParts[0];
                var cookieValue = cookieParts[1];
                
                this.Cookies.Add(new HttpCookie(cookieName, cookieValue, true));
            }
        }

        private void ParseRequestParameters(string formData)
        {
            this.ParseQueryParameters();
            this.ParseFormDataParameters(formData);
        }

        private void ParseHeaders(IEnumerable<string> requestContent)
        {
            var headers = requestContent.Skip(1).ToArray();

            FillHeadersData(headers);

            if (!this.Headers.ContainsHeader(GlobalConstants.HostHeaderKey))
            {
                throw new BadRequestException();
            }
        }

        private void FillHeadersData(IEnumerable<string> headers)
        {
            foreach (var header in headers)
            {
                if (string.IsNullOrEmpty(header)) break;

                var headerElements = header.Split(": ", StringSplitOptions.RemoveEmptyEntries);

                var headerKey = headerElements[0];
                var headerValue = headerElements[1];

                var currentHeader = new HttpHeader(headerKey, headerValue);

                this.Headers.Add(currentHeader);
            }
        }

        private void ParseRequestMethod(IReadOnlyList<string> requestLine)
        {
            var method = requestLine[0].Capitalize();

            this.RequestMethod = 
                Enum.TryParse<HttpRequestMethod>(method, out var parsedMethod) ? this.RequestMethod = parsedMethod : throw new BadRequestException();
        }

        private void ParseQueryParameters()
        {
            if (!this.Url.Contains('?')) return;

            var queryString = this.Url.Split('?','#')[1];

            if (string.IsNullOrWhiteSpace(queryString)) return;

            var queryParameters = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestQueryString(queryString, queryParameters))
                throw new BadRequestException();

            FillQueryParameters(queryParameters);
        }

        private void FillQueryParameters(IEnumerable<string> queryParameters)
        {
            foreach (var parameter in queryParameters)
            {
                var parameterParts = parameter.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parameterParts.Length != 2) continue;

                var parameterKey = WebUtility.UrlDecode(parameterParts[0]);
                var parameterValue = WebUtility.UrlDecode(parameterParts[1]);

                this.QueryData[parameterKey] = parameterValue;
            }
        }

        private void ParseFormDataParameters(string formData)
        {
            if (string.IsNullOrEmpty(formData)) return;

            var parameters = formData.Split('&', StringSplitOptions.RemoveEmptyEntries);

            FillFormData(parameters);
        }

        private void FillFormData(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
            {
                var parameterParts = parameter.Split('=');

                var parameterKey = parameterParts[0];
                var parameterValue = parameterParts[1];

                this.FormData[parameterKey] = parameterValue;
            }
        }

        private bool IsValidRequestLine(IReadOnlyList<string> requestLine)
        {
            var protocol = requestLine[2];
            var protocolIsValid = protocol == GlobalConstants.HttpProtocolFragment;
            var requestLineLengthIsValid = requestLine.Count == 3;

            return requestLineLengthIsValid && protocolIsValid;
        }

        private bool IsValidRequestQueryString(string queryString, IReadOnlyCollection<string> queryParameters)
        {
            var queryStringIsValid = !string.IsNullOrEmpty(queryString);
            var queryParametersAreValid = queryParameters.Count > 0;

            return queryStringIsValid && queryParametersAreValid;
        }

        private void ParseRequestUrl(IReadOnlyList<string> requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            this.Path = this.Url.Split('?','#', StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}