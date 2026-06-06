using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Sessions;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class NetcodeSessionLifecycleBridge : MonoBehaviour
    {
        private NetworkManager networkManager;

        private void OnEnable()
        {
            networkManager = NetworkManager.Singleton ?? GetComponent<NetworkManager>();
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void OnDisable()
        {
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            if (networkManager == null || clientId != networkManager.LocalClientId)
            {
                return;
            }

            if (!ServiceRegistry.TryResolve<ISessionService>(out var sessionService))
            {
                return;
            }

            sessionService.NotifyTransportDisconnected();
            EvaLog.Info("Local client disconnected from Netcode transport.");
        }
    }
}
