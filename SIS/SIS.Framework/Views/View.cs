using System.Collections.Generic;
using System.Linq;

namespace SIS.Framework.Views
{
    using System.IO;

    using ActionResults.Contracts;

    public class View : IRenderable
    {
        private const string RootDirectory = "../../../";

        private readonly string fullyQualifiedTemplateName;

        private readonly IDictionary<string, object> viewData;

        public View(string fullyQualifiedTemplateName, IDictionary<string, object> viewData)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
            this.viewData = viewData;
        }

        private string ReadFile(string templateName)
        {
            var path = $"{RootDirectory}{templateName}";

            var viewExists = File.Exists(path);

            if(!viewExists) throw new FileNotFoundException();
            
            var htmlText = File.ReadAllText(path);

            return htmlText;
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
                renderHtml = this.viewData.Aggregate(renderHtml, (current, parameter) => 
                    current.Replace($"{{{{{{{parameter.Key}}}}}}}", parameter.Value.ToString()));
            }

            return renderHtml;
        }
    }
}
