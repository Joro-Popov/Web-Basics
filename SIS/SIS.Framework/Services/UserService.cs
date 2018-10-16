namespace SIS.Framework.Services
{
    using Contracts;
    using HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class UserService : IUserService
    {
        private readonly ICookieService cookieService;

        public UserService(ICookieService cookieService)
        {
            this.cookieService = cookieService;
        }
        
        public bool IsAuthenticated(IHttpRequest request)
        {
            return request.Session.ContainsParameter("username") && request.Cookies.ContainsCookie(".auth-IRunes");
        }

        public void Authenticate(string username, IHttpResponse response, IHttpRequest request)
        {
            var cookieContent = this.cookieService.SetUserCookie(username);
            var cookie = new HttpCookie(".auth-IRunes", cookieContent, 7);

            response.Cookies.Add(cookie);
            request.Session.AddParameter("username", username);
        }

        public void Logout(IHttpRequest request)
        {
            var cookie = request.Cookies.GetCookie(".auth-IRunes");
            
            cookie.Delete();

            //response.Cookies.Add(cookie);
        }

        public void Login()
        {

        }
    }
}
