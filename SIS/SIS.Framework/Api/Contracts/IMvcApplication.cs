using SIS.Framework.Services.Contracts;

namespace SIS.Framework.Api.Contracts
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IServiceCollection dependencyContainer);
    }
}
