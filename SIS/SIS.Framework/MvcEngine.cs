namespace SIS.Framework
{
    using System;
    using System.Reflection;

    using WebServer;

    public static class MvcEngine
    {
        private const string CONTROLLERS_FOLDER = "Controllers";

        private const string CONTROLLER_SUFFIX = "Controller";

        private const string VIEWS_FOLDER = "Views";

        private const string MODELS_FOLDER = "Models";
        
        public static void Run(WebServer server)
        {
            RegisterAssemblyName();
            RegisterControllersData();
            RegisterViewsData();
            RegisterModelsData();

            try
            {
                server.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RegisterModelsData()
        {
            MvcContext.Get.ModelsFolder = MODELS_FOLDER;
        }

        private static void RegisterViewsData()
        {
            MvcContext.Get.ViewsFolder = VIEWS_FOLDER;
        }

        private static void RegisterControllersData()
        {
            MvcContext.Get.ControllersFolder = CONTROLLERS_FOLDER;
            MvcContext.Get.ControllersSuffix = CONTROLLER_SUFFIX;
        }

        private static void RegisterAssemblyName()
        {
            MvcContext.Get.AssemblyName = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
