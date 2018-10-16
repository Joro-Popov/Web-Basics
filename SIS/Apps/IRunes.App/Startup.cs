﻿namespace IRunes.App
{
    using SIS.WebServer;
    using SIS.Framework.Routers;
    using SIS.Framework;
    using SIS.Framework.Services;
    using SIS.Framework.Services.Contracts;

    public class Startup
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var server = new WebServer(80, new ControllerRouter(services));

            MvcEngine.Run(server);
        }

        private void Configure()
        {

        }

        private static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<ICookieService, CookieService>();
            collection.AddService<IUserService, UserService>();
        }
    }
}
