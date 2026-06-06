using Evaverse.Core.Runtime.App;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    /// <summary>
    /// Bridges <see cref="ISessionService"/> to Unity Netcode (Listen-server host + UDP client).
    /// </summary>
    public sealed class NetcodeSessionService : ISessionService, IInitializableService
    {
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public string LastJoinCode { get; private set; }

        public void Initialize()
        {
            EvaLog.Info("Netcode session service initialized.");
        }

        public void Shutdown()
        {
            Disconnect();
        }

        public void StartHost(SessionConfig config)
        {
            if (!TryGetBootstrap(out NetcodeBootstrap bootstrap, out NetworkManager nm))
            {
                EvaLog.Error($"{nameof(NetcodeSessionService)}: Netcode components missing. Add NetworkManager + UnityTransport + {nameof(NetcodeBootstrap)}.");
                return;
            }

            ushort port = config != null ? config.ListenPort : (ushort)7777;
            string publishAddress = config != null ? config.ClientDefaultAddress : "127.0.0.1";
            LastJoinCode = $"{publishAddress}:{port}";
            bootstrap.TryRegisterPlayerPrefab();
            bootstrap.ApplyHostListen(port);

            bool ok = nm.StartHost();
            if (!ok)
            {
                EvaLog.Error("Netcode StartHost failed.");
                IsHosting = false;
                IsConnected = false;
                LastJoinCode = null;
                return;
            }

            IsHosting = true;
            IsConnected = true;
            EvaLog.Info($"Netcode host started on UDP {port}.");
        }

        public void StartClient(string joinCode)
        {
            if (!TryParseDialTarget(joinCode, out string address, out ushort port))
            {
                EvaLog.Error($"{nameof(NetcodeSessionService)}: invalid join target '{joinCode}'. Use host:port or ip.");
                return;
            }

            LastJoinCode = joinCode;
            if (!TryGetBootstrap(out NetcodeBootstrap bootstrap, out NetworkManager nm))
            {
                EvaLog.Error($"{nameof(NetcodeSessionService)}: Netcode components missing.");
                return;
            }

            bootstrap.TryRegisterPlayerPrefab();
            bootstrap.ApplyClientDial(address, port);

            bool ok = nm.StartClient();
            if (!ok)
            {
                EvaLog.Error("Netcode StartClient failed.");
                IsHosting = false;
                IsConnected = false;
                LastJoinCode = null;
                return;
            }

            IsHosting = false;
            IsConnected = true;
            EvaLog.Info($"Netcode client dialing {address}:{port}.");
        }

        public void Disconnect()
        {
            NetworkManager nm = NetworkManager.Singleton;
            if (nm != null && (nm.IsServer || nm.IsClient))
            {
                nm.Shutdown();
            }

            IsHosting = false;
            IsConnected = false;
            LastJoinCode = null;
            EvaLog.Info("Netcode disconnected.");
        }

        private static bool TryParseDialTarget(string raw, out string address, out ushort port)
        {
            address = "127.0.0.1";
            port = 7777;

            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            string trimmed = raw.Trim();
            int colon = trimmed.LastIndexOf(':');
            if (colon > 0)
            {
                string host = trimmed.Substring(0, colon);
                string p = trimmed.Substring(colon + 1);
                if (ushort.TryParse(p, out ushort parsed))
                {
                    address = host;
                    port = parsed;
                    return true;
                }
            }

            if (trimmed.Contains('.'))
            {
                address = trimmed;
                return true;
            }

            return false;
        }

        private static bool TryGetBootstrap(out NetcodeBootstrap bootstrap, out NetworkManager manager)
        {
            manager = NetworkManager.Singleton;
            if (manager != null && manager.TryGetComponent(out bootstrap))
            {
                return true;
            }

            bootstrap = Object.FindFirstObjectByType<NetcodeBootstrap>();
            manager = bootstrap != null ? bootstrap.GetComponent<NetworkManager>() : null;
            return bootstrap != null && manager != null;
        }
    }
}
