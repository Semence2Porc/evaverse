using Evaverse.Core.Runtime.App;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Evaverse.Networking.Runtime
{
    /// <summary>
    /// Configures <see cref="NetworkManager"/> + <see cref="UnityTransport"/> on the same GameObject
    /// and registers the default networked player prefab.
    /// </summary>
    [DefaultExecutionOrder(-80)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkManager), typeof(UnityTransport))]
    public sealed class NetcodeBootstrap : MonoBehaviour
    {
        public const string ResourcesPlayerPrefabPath = "Prefabs/EvaverseNetworkPlayer";

        [SerializeField] private GameObject playerPrefab;

        private bool prefabHookAttempted;

        private void Reset()
        {
            if (!gameObject.TryGetComponent(out NetworkManager _))
            {
                gameObject.AddComponent<NetworkManager>();
            }

            if (!gameObject.TryGetComponent(out UnityTransport _))
            {
                gameObject.AddComponent<UnityTransport>();
            }
        }

        private void Awake()
        {
            var nm = GetComponent<NetworkManager>();
            var utp = GetComponent<UnityTransport>();
            EnsureNetworkConfig(nm);
            nm.NetworkConfig.NetworkTransport = utp;

            EvaLog.Info($"{nameof(NetcodeBootstrap)}: transport ready on {gameObject.name}.", this);
        }

        private void Start()
        {
            TryRegisterPlayerPrefab();
        }

        public void ApplyHostListen(ushort port)
        {
            var utp = GetComponent<UnityTransport>();
            utp.SetConnectionData("0.0.0.0", port);
            EvaLog.Info($"Netcode host listen {port} UDP.", this);
        }

        public void ApplyClientDial(string address, ushort port)
        {
            var utp = GetComponent<UnityTransport>();
            utp.SetConnectionData(address, port);
            EvaLog.Info($"Netcode client dialing {address}:{port}.", this);
        }

        public void TryRegisterPlayerPrefab()
        {
            if (prefabHookAttempted)
            {
                return;
            }

            prefabHookAttempted = true;

            GameObject prefab = playerPrefab != null ? playerPrefab : Resources.Load<GameObject>(ResourcesPlayerPrefabPath);
            if (prefab == null)
            {
                EvaLog.Warning(
                    $"No network player prefab. Use menu 'Evaverse/Networking/Bake Network Player Prefab' or assign {nameof(playerPrefab)}.",
                    this);
                return;
            }

            var nm = GetComponent<NetworkManager>();
            EnsureNetworkConfig(nm);
            nm.NetworkConfig.PlayerPrefab = prefab;
            try
            {
                nm.AddNetworkPrefab(prefab);
            }
            catch
            {
                // Already registered (e.g. re-entering play mode hot paths) — safe to ignore for prototype.
            }
        }

        private static void EnsureNetworkConfig(NetworkManager nm)
        {
            if (nm.NetworkConfig == null)
            {
                nm.NetworkConfig = new NetworkConfig();
            }
        }
    }
}
