using System;

namespace SIS.Framework.Views
{
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    using ActionResults.Contracts;

    public class View : IRenderable
    {
        //private const string RELATIVE_PATH = "../../../";
        
        //private const string LAYOUT_PATH = "Views/_Layout.html";

        //private const string RENDER_BODY = "@RenderBody";


        private readonly string fullHtmlContent;
        
        //private readonly IDictionary<string, object> viewData;
        
        public View(string fullHtmlContent/*, IDictionary<string, object> viewData*/) 
        {
            this.fullHtmlContent = fullHtmlContent;
            //this.viewData = viewData;
        }

        public string Render() => this.fullHtmlContent;

        //private string ReadFile(string templateName)
        //{
        //    var path = $"{RELATIVE_PATH}{templateName}";

        //    if (!File.Exists(path)) throw new FileNotFoundException();

        //    var htmlText = File.ReadAllText(path);

        //    var layout = File.ReadAllText($"{RELATIVE_PATH}{LAYOUT_PATH}");

        //    layout = layout.Replace(RENDER_BODY, htmlText);

        //    return layout;
        //}

        //public string Render()
        //{
        //    var fullHtml = this.ReadFile(this.fullHtmlContent);
        //    var renderHtml = this.RenderHtml(fullHtml);

        //    return renderHtml;
        //}

        //private string RenderHtml(string fullHtml)
        //{
        //    var renderHtml = fullHtml;

        //    if (this.viewData.Any())
        //    {
        //        foreach (var parameter in this.viewData)
        //        {
        //            renderHtml = renderHtml.Replace($"{{{{{{{parameter.Key}}}}}}}", parameter.Value.ToString());
        //        }
        //    }

        //    return renderHtml;
        //}
    }
}
