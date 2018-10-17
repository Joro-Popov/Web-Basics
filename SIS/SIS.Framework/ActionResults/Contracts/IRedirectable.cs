namespace SIS.Framework.ActionResults.Contracts
{
    using Base;

    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}
