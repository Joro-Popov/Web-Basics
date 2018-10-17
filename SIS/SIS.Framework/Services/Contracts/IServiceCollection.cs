namespace SIS.Framework.Services.Contracts
{
    using System;

    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        object CreateInstance(Type type);
    }
}
