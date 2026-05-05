using Evaverse.Core.Runtime.App;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class LocalSessionService : ISessionService, IInitializableService
    {
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }

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
            EvaLog.Info($"Started local host session '{config.SessionName}' for up to {config.MaxPlayers} players.");
        }

        public void StartClient(string joinCode)
        {
            IsHosting = false;
            IsConnected = true;
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
            EvaLog.Info("Disconnected local session.");
        }
    }
}
