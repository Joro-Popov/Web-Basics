namespace SIS.Framework.Views
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ViewEngine
    {
        private const string LAYOUT_DOES_NOT_EXIST = "layout View does not exist.";

        private const string ERROR_VIEW_DOES_NOT_EXIST = "Error View does not exist.";

        private const string VIEW_DOES_NOT_EXIST = "View does not exist at {0}";

        private const string VIEW_PATH_PREFIX = @"../../../";

        private const string DISPLAY_TEMPLATE_SUFFIX = "DisplayTemplate";

        private const string LAYOUT_VIEW_NAME = "_Layout";

        private const string ERROR_VIEW_NAME = "_Error";

        private const string VIEW_EXTENSION = "html";

        private const string MODEL_COLLECTION_VIEW_PARAMETER_PATTERN = @"\@Model\.Collection\.(\w+)\((.+)\)";
        
        // ../../../Views/
        private string ViewsFolderPath => $@"{VIEW_PATH_PREFIX}/{MvcContext.Get.ViewsFolder}/";

        // ../../../Views/Shared/
        private string ViewsSharedFolderPath => $@"{this.ViewsFolderPath}Shared/";

        // ../../../Views/Shared/DisplayTemplates/
        private string ViewsDisplayTemplateFolderPath => $@"{this.ViewsSharedFolderPath}/DisplayTemplates/";

        // ../../../Views/Shared/_Layout.html
        private string FormatLayoutViewPath() =>
            $@"{this.ViewsSharedFolderPath}{LAYOUT_VIEW_NAME}.{VIEW_EXTENSION}";

        // ../../../Views/Shared/_Error.html
        private string FormatErrorViewPath()
            => $@"{this.ViewsSharedFolderPath}/{ERROR_VIEW_NAME}.{VIEW_EXTENSION}";

        // ../../../Views/Home/Index.html
        private string FormatViewPath(string controllerName, string actionName) =>
            $@"{this.ViewsFolderPath}{controllerName}/{actionName}.{VIEW_EXTENSION}";

        // ../../../Views/Shared/DisplayTemplates/LoginViewModelDisplayTemplate.html
        private string FormatDisplayTemplatePath(string objectName) =>
            $@"{this.ViewsDisplayTemplateFolderPath}{objectName}{DISPLAY_TEMPLATE_SUFFIX}.{VIEW_EXTENSION}";


        private string ReadLayoutHtml(string layoutViewPath)
        {
            if (!File.Exists(layoutViewPath))
            {
                throw new FileNotFoundException(LAYOUT_DOES_NOT_EXIST);
            }

            return File.ReadAllText(layoutViewPath);
        }

        private string ReadErrorHtml(string errorViewPath)
        {
            if (!File.Exists(errorViewPath))
            {
                throw new FileNotFoundException(ERROR_VIEW_DOES_NOT_EXIST);
            }

            return File.ReadAllText(errorViewPath);
        }

        private string ReadViewHtml(string viewPath)
        {
            if (!File.Exists(viewPath))
            {
                throw new FileNotFoundException(string.Format(VIEW_DOES_NOT_EXIST, viewPath));
            }

            return File.ReadAllText(viewPath);
        }

        private string RenderObject(object viewObject, string displayTemplate)
        {
            foreach (var property in viewObject.GetType().GetProperties())
            {
                displayTemplate = this.RenderViewData(displayTemplate, property.GetValue(viewObject), property.Name);
            }

            return displayTemplate;
        }

        private string RenderViewData(string template, object viewObject, string viewObjectName = null)
        {
            // render collection
            if (viewObject != null
                && viewObject.GetType() != typeof(string)
                && viewObject is IEnumerable enumerable
                && Regex.IsMatch(template, MODEL_COLLECTION_VIEW_PARAMETER_PATTERN))
            {
                var collectionMatch = Regex.Matches(template, MODEL_COLLECTION_VIEW_PARAMETER_PATTERN)
                    .First(cm => cm.Groups[1].Value == viewObjectName);

                var fullMatch = collectionMatch.Groups[0].Value;
                var itemPattern = collectionMatch.Groups[2].Value;

                var result = string.Empty;

                foreach (var subObject in enumerable)
                {
                    result += itemPattern.Replace("@Item", this.RenderViewData(template, subObject));
                }

                return template.Replace(fullMatch, result);
            }

            // render object
            if (viewObject != null
                && !viewObject.GetType().IsPrimitive
                && viewObject.GetType() != typeof(string))
            {
                if (File.Exists(this.FormatDisplayTemplatePath(viewObject.GetType().Name)))
                {
                    var displayTemplate = File.ReadAllText(this.FormatDisplayTemplatePath(viewObject.GetType().Name));

                    var renderObject = this.RenderObject(viewObject, displayTemplate);

                    return viewObjectName != null
                        ? template.Replace($"@Model.{viewObjectName}", renderObject)
                        : renderObject;
                }
            }

            // render primitive
            return viewObjectName != null
                ? template.Replace($@"@Model.{viewObjectName}", viewObject?.ToString())
                : viewObject?.ToString();
        }


        public string GetErrorContent() =>
            this.ReadLayoutHtml(this.FormatLayoutViewPath())
                .Replace("@RenderError()", this.ReadErrorHtml(this.FormatErrorViewPath()));

        public string GetViewContent(string controllerName, string actionName) =>
            this.ReadLayoutHtml(this.FormatLayoutViewPath())
                .Replace("@RenderBody()", this.ReadViewHtml(this.FormatViewPath(controllerName, actionName)));

        public string RenderHtml(string fullHtmlContent, IDictionary<string, object> viewData)
        {
            var renderedHtml = fullHtmlContent;

            if (viewData.Count > 0)
            {
                foreach (var parameter in viewData)
                {
                    renderedHtml = this.RenderViewData(renderedHtml, parameter.Value, parameter.Key);
                }
            }

            if (viewData.ContainsKey("Error"))
            {
                renderedHtml = renderedHtml.Replace("@Error", viewData["Error"].ToString());
            }

            return renderedHtml;
        }
    }
}
