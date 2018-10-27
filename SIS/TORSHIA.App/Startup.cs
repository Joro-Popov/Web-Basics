namespace TORSHIA.App
{
    using SIS.Framework.Api;
    using SIS.Framework.Services;
    using SIS.Framework.Services.Contracts;

    public class Startup : MvcApplication
    {
        public override void ConfigureServices(IServiceCollection dependencyContainer)
        {
            dependencyContainer.AddService<IHashService, HashService>();
        }
    }
}
