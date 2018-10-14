namespace SIS.Framework
{
    using System;
    using System.Reflection;

    using WebServer;

    public static class MvcEngine
    {
        private const string ControllersFolder = "Controllers";
        private const string ControllersSuffix = "Controller";
        private const string ViewsFolder = "Views";
        private const string ModelsFolder = "Models";
        
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
                // TODO: Log Errors
                Console.WriteLine(e.Message);
            }
        }

        private static void RegisterModelsData()
        {
            MvcContext.Get.ModelsFolder = ModelsFolder;
        }

        private static void RegisterViewsData()
        {
            MvcContext.Get.ViewsFolder = ViewsFolder;
        }

        private static void RegisterControllersData()
        {
            MvcContext.Get.ControllersFolder = ControllersFolder;
            MvcContext.Get.ControllersSuffix = ControllersSuffix;
        }

        private static void RegisterAssemblyName()
        {
            MvcContext.Get.AssemblyName = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
