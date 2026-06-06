using Evaverse.Core.Runtime.App;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class LocalSessionService : ISessionService, IInitializableService
    {
        public SessionBackend Backend => SessionBackend.Local;
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsBusy => false;
        public string JoinCode { get; private set; } = string.Empty;
        public string StatusMessage { get; private set; } = "Local session idle.";

        public void Initialize()
        {
            EvaLog.Info("Local session service initialized.");
        }

        public void Shutdown()
        {
            Disconnect();
        }

        public void StartHost(SessionConfig config)
        {
            IsHosting = true;
            IsConnected = true;
            JoinCode = "local";
            StatusMessage = "Local host session active.";
            EvaLog.Info($"Started local host session '{config.SessionName}' for up to {config.MaxPlayers} players.");
        }

        public void StartClient(string joinCode)
        {
            IsHosting = false;
            IsConnected = true;
            JoinCode = joinCode;
            StatusMessage = "Local client session active.";
            EvaLog.Info($"Connected local client with join code '{joinCode}'.");
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            IsHosting = false;
            IsConnected = false;
            JoinCode = string.Empty;
            StatusMessage = "Local session idle.";
            EvaLog.Info("Disconnected local session.");
        }
    }
}
