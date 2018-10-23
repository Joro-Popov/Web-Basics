using System;

namespace SIS.Framework.Views
{
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    using ActionResults.Contracts;

    public class View : IRenderable
    {
        private readonly string fullHtmlContent;
        
        public View(string fullHtmlContent) 
        {
            this.fullHtmlContent = fullHtmlContent;
        }

        public string Render() => this.fullHtmlContent;
    }
}
