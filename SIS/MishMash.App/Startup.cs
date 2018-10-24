namespace MishMash.App
{
    using SIS.Framework.Api;
    using SIS.Framework.Logger;
    using SIS.Framework.Logger.Contracts;
    using SIS.Framework.Services;
    using SIS.Framework.Services.Contracts;

    public class Startup : MvcApplication
    {
        public override void ConfigureServices(IServiceCollection dependencyContainer)
        {
            dependencyContainer.AddService<IHashService, HashService>();
            dependencyContainer.AddService<IConsoleLogger, ConsoleLogger>();
        }
    }
}
