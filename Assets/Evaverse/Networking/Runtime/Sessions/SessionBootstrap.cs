using Evaverse.Core.Runtime.App;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public enum SessionBackend
    {
        Local,
        Netcode,
        MultiplayerRelay
    }

    /// <summary>
    /// Registers a session implementation with <see cref="ServiceRegistry"/> at runtime.
    /// </summary>
    public sealed class SessionBootstrap : MonoBehaviour
    {
        [SerializeField] private SessionBackend backend = SessionBackend.Local;
        [SerializeField] private bool registerDebugNetcodeHud = true;

        private ISessionService sessionService;

        private void Awake()
        {
            sessionService = backend == SessionBackend.Netcode
                ? new NetcodeSessionService()
                : backend == SessionBackend.MultiplayerRelay
                    ? new MultiplayerRelaySessionService()
                : new LocalSessionService();

            sessionService.As<IInitializableService>()?.Initialize();
            ServiceRegistry.Register(sessionService);

            if (TryGetComponent(out EvaverseSessionDebugHud hud))
            {
                hud.enabled = backend != SessionBackend.Local && registerDebugNetcodeHud;
            }
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
