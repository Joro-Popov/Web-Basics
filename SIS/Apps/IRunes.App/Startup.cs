namespace IRunes.App
{
    using SIS.WebServer;
    using SIS.Framework.Routers;
    using SIS.Framework;
    using SIS.Framework.Services;
    using SIS.Framework.Services.Contracts;
    using SIS.Framework.Logger;
    using SIS.Framework.Logger.Contracts;

    public class Startup
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var controllerRouter = new ControllerRouter(services);
            var resourceRouter = new ResourceRouter();

            var router = new HttpHandlerContext(controllerRouter, resourceRouter);

            var server = new WebServer(80, router);

            MvcEngine.Run(server);
        }

        private void Configure()
        {

        }

        private static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IConsoleLogger, ConsoleLogger>();
        }
    }
}
