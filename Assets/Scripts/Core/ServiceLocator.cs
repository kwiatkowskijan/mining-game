using System;
using System.Collections.Generic;

namespace MiningGame.Core
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, Object> services = new Dictionary<Type, Object>();

        public static void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public static T Get<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }

            throw new Exception($"Service not found: {typeof(T)}");
        }

        public static void Unregister<T>() where T : class
        {
            services.Remove(typeof(T));
        }
    }
}