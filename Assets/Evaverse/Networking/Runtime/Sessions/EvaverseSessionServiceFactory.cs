using Evaverse.Networking.Runtime.Netcode;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public static class EvaverseSessionServiceFactory
    {
        public static ISessionService Create(SessionBackend backend, NetcodeBootstrap bootstrap, MonoBehaviour coroutineHost)
        {
            switch (backend)
            {
                case SessionBackend.DirectNetcode:
                    return new DirectNetcodeSessionService(bootstrap);
                case SessionBackend.MultiplayerRelay:
                    return new RelaySessionService(bootstrap, coroutineHost);
                default:
                    return new LocalSessionService();
            }
        }
    }
}
