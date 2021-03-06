﻿namespace SIS.Framework.Services.Contracts
{
    using System;

    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        T CreateInstance<T>();

        object CreateInstance(Type type);
    }
}
