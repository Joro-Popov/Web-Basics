namespace SIS.Framework.Views
{
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    using ActionResults.Contracts;

    public class View : IRenderable
    {
        private const string ROOT_DIRECTORY = "../../../";

        private const string LAYOUT_LOGGED = "Views/_LayoutLogged.html";

        private const string LAYOUT_LOGGED_OUT = "Views/_Layout.html";

        private const string RENDER_BODY = "@RenderBody";


        private readonly string fullyQualifiedTemplateName;
        
        private readonly IDictionary<string, object> viewData;

        private readonly bool IsLogged;
        
        public View(string fullyQualifiedTemplateName, IDictionary<string, object> viewData, bool isLogged) 
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
            this.viewData = viewData;
            this.IsLogged = isLogged;
        }

        private string ReadFile(string templateName)
        {
            var path = $"{ROOT_DIRECTORY}{templateName}";
            
            if(!File.Exists(path)) throw new FileNotFoundException();
            
            var htmlText = File.ReadAllText(path);

            string layout;

            layout = File.ReadAllText(this.IsLogged ? $"{ROOT_DIRECTORY}{LAYOUT_LOGGED}" : $"{ROOT_DIRECTORY}{LAYOUT_LOGGED_OUT}");

            layout = layout.Replace(RENDER_BODY, htmlText);

            return layout;
        }

        public string Render()
        {
            var fullHtml = this.ReadFile(this.fullyQualifiedTemplateName);
            var renderHtml = this.RenderHtml(fullHtml);

            return renderHtml;
        }

        private string RenderHtml(string fullHtml)
        {
            var renderHtml = fullHtml;
            
            if (this.viewData.Any())
            {
                foreach (var parameter in this.viewData)
                {
                    renderHtml = renderHtml.Replace($"{{{{{{{parameter.Key}}}}}}}", parameter.Value.ToString());
                }
            }

            return renderHtml;
        }
    }
}
