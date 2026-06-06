using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Netcode;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    /// <summary>
    /// Registers a session implementation with <see cref="ServiceRegistry"/> at runtime.
    /// </summary>
    public sealed class SessionBootstrap : MonoBehaviour
    {
        [SerializeField] private SessionBackend backend = SessionBackend.Local;
        [SerializeField] private SessionConfig sessionConfig;

        private ISessionService sessionService;

        public SessionBackend Backend => backend;

        private void Awake()
        {
            NetcodeBootstrap bootstrap = GetComponent<NetcodeBootstrap>();
            if (backend != SessionBackend.Local && bootstrap == null)
            {
                EvaLog.Error($"Session backend '{backend}' requires a {nameof(NetcodeBootstrap)} on the same GameObject.");
            }

            sessionService = EvaverseSessionServiceFactory.Create(backend, bootstrap, this);
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
