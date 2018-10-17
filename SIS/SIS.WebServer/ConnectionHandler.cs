namespace SIS.WebServer
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Linq;

    using HTTP.Enums;
    using HTTP.Requests;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Cookies;
    using HTTP.Exceptions;
    using HTTP.Sessions;
    using HTTP.Common;
    using API;
    using Results;
    
    public class ConnectionHandler
    {
        private readonly Socket client;
        private readonly IHttpHandler handler;

        public ConnectionHandler(Socket client, IHttpHandler handler)
        {
            this.client = client;
            this.handler = handler;
        }
        
        private async Task<IHttpRequest> ReadRequestAsync()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                var numberOfBytesRead = await client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0) break;

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);

                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023) break;
            }

            return result.Length == 0 ? null : new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            return this.handler.Handle(httpRequest);
        }

        private bool IsResourceRequest(IHttpRequest httpRequest)
        {
            if (string.IsNullOrWhiteSpace(httpRequest.Path.Split('/').Last())) return false;

            var extension = Path.GetExtension(httpRequest.Path);

            return !string.IsNullOrWhiteSpace(extension) && GlobalConstants.FileExtensions.Contains(extension.Substring(1));
        }

        private async Task PrepareResponseAsync(IHttpResponse httpResponse)
        {
            var byteSegment = httpResponse.GetBytes();
            await this.client.SendAsync(byteSegment, SocketFlags.None);
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;
                
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpRequest request, IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId == null ) return;
            
            HttpCookie cookie = null;

            if (request.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                if (Guid.TryParse(sessionId, out Guid guidOutput)) return;

                sessionId = Guid.NewGuid().ToString();

                cookie = new HttpCookie(HttpSessionStorage.SessionCookieKey, $"{sessionId}");

                httpResponse.AddCookie(cookie);

                return;
            }

            cookie = new HttpCookie(HttpSessionStorage.SessionCookieKey, $"{sessionId}");

            httpResponse.AddCookie(cookie);
        }

        public async Task ProcessRequestAsync()
        {
            var httpRequest = await ReadRequestAsync();
            
            if (httpRequest != null)
            {
                try
                {
                    var sessionId = this.SetRequestSession(httpRequest);

                    var httpResponse = this.HandleRequest(httpRequest);

                    this.SetResponseSession(httpRequest, httpResponse, sessionId);

                    await this.PrepareResponseAsync(httpResponse);
                }
                catch (BadRequestException e)
                {
                    await this.PrepareResponseAsync(new TextResult(e.Message, HttpResponseStatusCode.BadRequest));
                }
                catch (Exception e)
                {
                    await this.PrepareResponseAsync(new TextResult(e.Message, HttpResponseStatusCode.InternalServerError));
                }
            }
            
            this.client.Shutdown(SocketShutdown.Both);
        }
    }
}