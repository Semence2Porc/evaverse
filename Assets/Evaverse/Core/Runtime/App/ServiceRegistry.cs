using System;
using System.Collections.Generic;

namespace Evaverse.Core.Runtime.App
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<TService>(TService service) where TService : class
        {
            Services[typeof(TService)] = service;
        }

        public static bool TryResolve<TService>(out TService service) where TService : class
        {
            if (Services.TryGetValue(typeof(TService), out var resolved))
            {
                service = resolved as TService;
                return service != null;
            }

            service = null;
            return false;
        }

        public static TService Resolve<TService>() where TService : class
        {
            if (TryResolve<TService>(out var service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service '{typeof(TService).Name}' has not been registered.");
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}
