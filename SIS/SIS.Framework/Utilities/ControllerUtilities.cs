namespace SIS.Framework.Utilities
{
    public static class ControllerUtilities
    {
        private const string FILE_EXTENSION = ".html";

        public static string GetControllerName(object controller) =>
            controller.GetType().Name.Replace(MvcContext.Get.ControllersSuffix, string.Empty);

        public static string GetViewFullQualifiedName(string controller, string action) =>
            $"{MvcContext.Get.ViewsFolder}/{controller}/{action}{FILE_EXTENSION}";
    }
}
