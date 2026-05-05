using Evaverse.Core.Runtime.App;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class SessionBootstrap : MonoBehaviour
    {
        private ISessionService sessionService;

        private void Awake()
        {
            sessionService = new LocalSessionService();
            sessionService.As<IInitializableService>()?.Initialize();
            ServiceRegistry.Register(sessionService);
        }

        private void OnDestroy()
        {
            sessionService.As<IInitializableService>()?.Shutdown();
        }
    }

    internal static class SessionBootstrapExtensions
    {
        public static TTarget As<TTarget>(this object instance) where TTarget : class
        {
            return instance as TTarget;
        }
    }
}
