namespace SIS.Framework.ActionResults.Contracts
{
    using Base;

    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
