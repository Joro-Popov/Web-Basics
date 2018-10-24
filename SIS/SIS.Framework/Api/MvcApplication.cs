namespace SIS.Framework.Api
{
    using Contracts;
    using SIS.Framework.Services.Contracts;

    public class MvcApplication : IMvcApplication
    {
        public virtual void Configure()
        {
        }

        public virtual void ConfigureServices(IServiceCollection dependencyContainer)
        {

        }
    }
}
