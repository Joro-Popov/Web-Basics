namespace SIS.Framework.Views
{
    using System.IO;

    using ActionResults.Contracts;

    public class View : IRenderable
    {
        private const string RootDirectory = "../../../";

        private readonly string fullyQualifiedTemplateName;
       
        public View(string fullyQualifiedTemplateName)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
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

            return fullHtml;
        }
    }
}
