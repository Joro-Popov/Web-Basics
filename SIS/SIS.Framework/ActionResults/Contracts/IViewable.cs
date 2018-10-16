using SIS.Framework.ActionResults.Contracts.Base;

namespace SIS.Framework.ActionResults.Contracts
{
    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
