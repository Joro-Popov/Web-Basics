namespace SIS.Framework.Services
{
    using Contracts;
    using HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class AuthenticationService : IAuthenticationService
    {
        private const string AUTH_COOKIE_NAME = ".auth";

        private readonly ICookieService cookieService;

        public AuthenticationService(ICookieService cookieService)
        {
            this.cookieService = cookieService;
        }
        
        public bool IsAuthenticated(IHttpRequest request)
        {
            return request.Session.ContainsParameter("username") && request.Cookies.ContainsCookie(AUTH_COOKIE_NAME);
        }

        public void Login(string username, IHttpResponse response, IHttpRequest request)
        {
            var cookieContent = this.cookieService.SetUserCookie(username);
            var cookie = new HttpCookie(AUTH_COOKIE_NAME, cookieContent, 7);

            response.Cookies.Add(cookie);
            request.Session.AddParameter("username", username);
        }

        public void Logout(IHttpRequest request, IHttpResponse response)
        {
            var cookie = request.Cookies.GetCookie(AUTH_COOKIE_NAME);
            
            cookie.Delete();

            response.Cookies.Add(cookie);
        }
    }
}
