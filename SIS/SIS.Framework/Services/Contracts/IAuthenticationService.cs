namespace SIS.Framework.Services.Contracts
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public interface IAuthenticationService
    {
        bool IsAuthenticated(IHttpRequest request);

        void Authenticate(string username, IHttpResponse response, IHttpRequest request);

        void Logout(IHttpRequest request, IHttpResponse response);
    }
}
