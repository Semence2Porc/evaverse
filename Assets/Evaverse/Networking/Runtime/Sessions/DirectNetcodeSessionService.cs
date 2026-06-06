using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Netcode;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class DirectNetcodeSessionService : ISessionService, IInitializableService
    {
        private readonly NetcodeBootstrap bootstrap;

        public DirectNetcodeSessionService(NetcodeBootstrap bootstrap)
        {
            this.bootstrap = bootstrap;
        }

        public SessionBackend Backend => SessionBackend.DirectNetcode;
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsBusy => false;
        public string JoinCode { get; private set; } = string.Empty;
        public string StatusMessage { get; private set; } = "Direct Netcode idle.";

        public void Initialize()
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                EvaLog.Error(StatusMessage);
                return;
            }

            JoinCode = NetcodeBootstrap.ResolveLocalJoinTarget(bootstrap.DirectPort);
            StatusMessage = "Direct Netcode ready. Host or paste join target.";
            EvaLog.Info("Direct Netcode session service initialized.");
        }

        public void Shutdown()
        {
            Disconnect();
        }

        public void StartHost(SessionConfig config)
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                return;
            }

            if (IsConnected)
            {
                Disconnect();
            }

            if (!bootstrap.TryStartDirectHost())
            {
                JoinCode = NetcodeBootstrap.ResolveLocalJoinTarget(bootstrap.DirectPort);
                StatusMessage = "Direct host failed to start.";
                return;
            }

            IsHosting = true;
            IsConnected = true;
            JoinCode = NetcodeBootstrap.ResolveLocalJoinTarget(bootstrap.DirectPort);
            StatusMessage = $"Hosting on {JoinCode}";
            EvaLog.Info($"Started direct Netcode host '{config.SessionName}' for up to {config.MaxPlayers} players.");
        }

        public void StartClient(string joinCode)
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                return;
            }

            if (IsConnected)
            {
                Disconnect();
            }

            string target = string.IsNullOrWhiteSpace(joinCode) ? JoinCode : joinCode.Trim();
            if (!bootstrap.TryStartDirectClient(target))
            {
                StatusMessage = $"Client failed to connect to {target}.";
                JoinCode = target;
                return;
            }

            IsHosting = false;
            IsConnected = true;
            JoinCode = target;
            StatusMessage = $"Connecting to {target}";
            EvaLog.Info($"Started direct Netcode client using join target '{target}'.");
        }

        public void Disconnect()
        {
            if (!IsConnected && bootstrap?.NetworkManager?.IsListening != true)
            {
                return;
            }

            bootstrap?.ShutdownNetwork();
            IsHosting = false;
            IsConnected = false;
            JoinCode = bootstrap != null ? NetcodeBootstrap.ResolveLocalJoinTarget(bootstrap.DirectPort) : string.Empty;
            StatusMessage = "Direct Netcode idle.";
            EvaLog.Info("Disconnected direct Netcode session.");
        }
    }
}
