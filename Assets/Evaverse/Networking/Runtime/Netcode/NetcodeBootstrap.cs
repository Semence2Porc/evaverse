using System;
using System.Net;
using System.Net.Sockets;
using Evaverse.Core.Runtime.App;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class NetcodeBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private ushort directPort = 7777;

        private NetworkManager networkManager;
        private UnityTransport transport;

        public NetworkManager NetworkManager => networkManager;
        public GameObject PlayerPrefab => playerPrefab;
        public ushort DirectPort => directPort;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            if (networkManager == null)
            {
                networkManager = gameObject.AddComponent<NetworkManager>();
            }

            transport = GetComponent<UnityTransport>();
            if (transport == null)
            {
                transport = gameObject.AddComponent<UnityTransport>();
            }

            networkManager.NetworkConfig.NetworkTransport = transport;
            DontDestroyOnLoad(gameObject);
        }

        public void SetPlayerPrefab(GameObject prefab)
        {
            playerPrefab = prefab;
        }

        public void PreparePlayerPrefab()
        {
            if (playerPrefab == null)
            {
                EvaLog.Warn("NetcodeBootstrap has no player prefab assigned.");
                return;
            }

            networkManager.NetworkConfig.PlayerPrefab = playerPrefab;
        }

        public bool TryConfigureDirectTransport(string address, ushort port)
        {
            if (transport == null)
            {
                return false;
            }

            transport.SetConnectionData(address, port);
            return true;
        }

        public bool TryStartDirectHost()
        {
            PreparePlayerPrefab();
            TryConfigureDirectTransport("0.0.0.0", directPort);

            if (!networkManager.StartHost())
            {
                EvaLog.Error("Direct Netcode host failed to start.");
                return false;
            }

            EvaLog.Info($"Direct Netcode host listening on port {directPort}.");
            return true;
        }

        public bool TryStartDirectClient(string joinTarget)
        {
            PreparePlayerPrefab();
            ParseJoinTarget(joinTarget, out string address, out ushort port);
            TryConfigureDirectTransport(address, port);

            if (!networkManager.StartClient())
            {
                EvaLog.Error($"Direct Netcode client failed to connect to {address}:{port}.");
                return false;
            }

            EvaLog.Info($"Direct Netcode client connecting to {address}:{port}.");
            return true;
        }

        public void ShutdownNetwork()
        {
            if (networkManager != null && networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }

        public static string ResolveLocalJoinTarget(ushort port)
        {
            string address = TryResolveLocalIpv4() ?? "127.0.0.1";
            return $"{address}:{port}";
        }

        private static void ParseJoinTarget(string joinTarget, out string address, out ushort port)
        {
            address = "127.0.0.1";
            port = 7777;

            if (string.IsNullOrWhiteSpace(joinTarget))
            {
                return;
            }

            string trimmed = joinTarget.Trim();
            int separatorIndex = trimmed.LastIndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= trimmed.Length - 1)
            {
                address = trimmed;
                return;
            }

            address = trimmed.Substring(0, separatorIndex);
            if (!ushort.TryParse(trimmed.Substring(separatorIndex + 1), out port))
            {
                port = 7777;
            }
        }

        private static string TryResolveLocalIpv4()
        {
            try
            {
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect("8.8.8.8", 65530);
                if (socket.LocalEndPoint is IPEndPoint endpoint)
                {
                    return endpoint.Address.ToString();
                }
            }
            catch (Exception exception)
            {
                EvaLog.Warn($"Could not resolve local IPv4 address: {exception.Message}");
            }

            return null;
        }
    }
}
