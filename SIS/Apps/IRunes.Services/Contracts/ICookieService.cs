namespace IRunes.Services.Contracts
{
    public interface ICookieService
    {
        string SetUserCookie(string username);

        string GetUserData(string cookieContent);
    }
}
