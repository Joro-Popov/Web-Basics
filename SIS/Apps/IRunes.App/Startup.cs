﻿namespace IRunes.App
{
    using SIS.Framework.Services;
    using SIS.Framework.Services.Contracts;
    using SIS.Framework.Logger;
    using SIS.Framework.Logger.Contracts;
    using SIS.Framework.Api;

    public class Startup : MvcApplication
    {
        public override void ConfigureServices(IServiceCollection dependencyContainer)
        {
            dependencyContainer.AddService<IHashService, HashService>();
            dependencyContainer.AddService<IConsoleLogger, ConsoleLogger>();
        }
    }
}
