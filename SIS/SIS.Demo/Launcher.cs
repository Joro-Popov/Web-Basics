namespace SIS.Demo
{
    using WebServer;

    using Framework;
    using Framework.Routers;
    using Framework.Services;
    using Framework.Services.Contracts;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var controllerRouter = new ControllerRouter(services); 

            var server = new WebServer(80, controllerRouter);

            MvcEngine.Run(server);
        }

        private void Configure()
        {

        }

        private static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<ICookieService, CookieService>();
        }
    }
}
