namespace SIS.Framework
{
    using Api.Contracts;
    using Routers;
    using Services;

    public static class WebHost
    {
        private const int HOSTING_PORT = 80;

        public static void Start(IMvcApplication application)
        {
            var container = new ServiceCollection();

            application.ConfigureServices(container);

            var controllerRouter = new ControllerRouter(container);
            var resourceRouter = new ResourceRouter();

            var router = new HttpHandlerContext(controllerRouter, resourceRouter);
            application.Configure();

            var server = new WebServer.WebServer(HOSTING_PORT, router);

            MvcEngine.Run(server);
        }
    }
}
