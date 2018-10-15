using System;

namespace SIS.Framework.Services.Contracts
{
    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        object CreateInstance(Type type);
    }
}
