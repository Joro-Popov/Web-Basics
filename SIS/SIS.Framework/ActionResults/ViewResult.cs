namespace SIS.Framework.ActionResults
{
    using Contracts;

    public class ViewResult : IViewable
    {
        public ViewResult(IRenderable view)
        {
            this.View = view;
        }

        public string Invoke() => this.View.Render();

        public IRenderable View { get; set; }
    }
}
