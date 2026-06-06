using System.Threading.Tasks;
using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime;
using Unity.Services.Multiplayer;

namespace Evaverse.Networking.Runtime.Sessions
{
    /// <summary>
    /// Session service backed by Unity Multiplayer Services SDK with Relay networking.
    /// Produces a join code that other peers can use to join the same session.
    /// </summary>
    public sealed class MultiplayerRelaySessionService : ISessionService, IInitializableService
    {
        private const string SessionType = "evaverse-hub";

        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public string LastJoinCode { get; private set; }

        private ISession session;

        public void Initialize()
        {
            EvaLog.Info("Multiplayer Relay session service initialized.");
        }

        public void Shutdown()
        {
            Disconnect();
        }

        public void StartHost(SessionConfig config)
        {
            _ = StartHostAsync(config);
        }

        public void StartClient(string joinCode)
        {
            _ = StartClientAsync(joinCode);
        }

        public void Disconnect()
        {
            if (session != null)
            {
                try
                {
                    session.LeaveAsync();
                }
                catch
                {
                    // Best effort.
                }

                session = null;
            }

            IsHosting = false;
            IsConnected = false;
            LastJoinCode = null;
            EvaLog.Info("Multiplayer session disconnected.");
        }

        private async Task StartHostAsync(SessionConfig config)
        {
            await UnityServicesBootstrap.EnsureInitializedAsync();

            int maxPlayers = config != null ? config.MaxPlayers : 8;

            try
            {
                SessionOptions options = new SessionOptions
                {
                    Type = SessionType,
                    Name = config != null ? config.SessionName : "Evaverse Session",
                    IsPrivate = config != null && config.PrivateSession,
                    MaxPlayers = maxPlayers
                }.WithRelayNetwork();

                if (!PrepareNetcode())
                {
                    return;
                }

                session = await MultiplayerService.Instance.CreateSessionAsync(options);
                IsHosting = true;
                IsConnected = true;
                LastJoinCode = session.Code;
                EvaLog.Info($"Relay session created. Join code: {session.Code}");
            }
            catch (System.Exception ex)
            {
                IsHosting = false;
                IsConnected = false;
                LastJoinCode = null;
                EvaLog.Error($"Relay StartHost failed: {ex.Message}");
            }
        }

        private async Task StartClientAsync(string joinCode)
        {
            await UnityServicesBootstrap.EnsureInitializedAsync();

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                EvaLog.Error("Relay StartClient failed: join code is empty.");
                return;
            }

            try
            {
                JoinSessionOptions options = new JoinSessionOptions
                {
                    Type = SessionType
                };

                if (!PrepareNetcode())
                {
                    return;
                }

                session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode.Trim(), options);
                IsHosting = false;
                IsConnected = true;
                LastJoinCode = joinCode.Trim();
                EvaLog.Info("Relay session joined.");
            }
            catch (System.Exception ex)
            {
                IsHosting = false;
                IsConnected = false;
                LastJoinCode = null;
                EvaLog.Error($"Relay StartClient failed: {ex.Message}");
            }
        }

        private static bool PrepareNetcode()
        {
            NetcodeBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<NetcodeBootstrap>();
            if (bootstrap == null)
            {
                EvaLog.Error($"{nameof(MultiplayerRelaySessionService)}: missing Netcode bootstrap. Rebuild the hub as a Relay or Netcode hub.");
                return false;
            }

            bootstrap.TryRegisterPlayerPrefab();
            return true;
        }
    }
}
