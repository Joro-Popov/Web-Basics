namespace SIS.Framework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    public class ServiceCollection : IServiceCollection
    {
        private readonly IDictionary<Type, Type> dependencyContainer;

        public ServiceCollection()
        {
            this.dependencyContainer = new Dictionary<Type, Type>();
        }

        public void AddService<TSource, TDestination>()
        {
            dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public T CreateInstance<T>() => (T)this.CreateInstance(typeof(T));

        public object CreateInstance(Type type)
        {
            var instanceType = this[type] ?? type;

            if (instanceType.IsInterface || instanceType.IsAbstract)
            {
                throw new Exception($"Type {type.FullName} cannot be instantiated!");
            }

            var constructor = instanceType.GetConstructors().First();
            var constructorParameters = constructor.GetParameters();

            var constructorParameterObjects = new List<object>();

            foreach (var constructorParameter in constructorParameters)
            {
                var parameterObject = this.CreateInstance(constructorParameter.ParameterType);

                constructorParameterObjects.Add(parameterObject);
            }

            var obj = constructor.Invoke(constructorParameterObjects.ToArray());

            return obj;
        }

        private Type this[Type key] =>
            this.dependencyContainer.ContainsKey(key) ? this.dependencyContainer[key] : null;
    }
}
