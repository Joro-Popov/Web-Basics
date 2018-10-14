namespace SIS.Framework.ActionResults
{
    using Contracts;

    public class RedirectResult : IRedirectable
    {
        public RedirectResult(string redirectUrl)
        {
            this.RedirectUrl = redirectUrl;
        }

        public string Invoke() => this.RedirectUrl;

        public string RedirectUrl { get; }
    }
}
