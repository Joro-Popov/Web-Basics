using System;
using System.Collections.Generic;
using System.Linq;
using SIS.Framework.Services.Contracts;

namespace SIS.Framework.Services
{
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

        public object CreateInstance(Type type)
        {
            if (dependencyContainer.ContainsKey(type))
            {
                type = dependencyContainer[type];
            }

            if (type.IsInterface || type.IsAbstract)
            {
                throw new Exception($"Type {type.FullName} cannot be instantiated!");
            }

            var constructor = type.GetConstructors().First();
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
    }
}
