using Evaverse.Core.Runtime.App;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class LocalSessionService : ISessionService, IInitializableService
    {
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public string LastJoinCode { get; private set; }

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
            LastJoinCode = null;
            string name = config != null ? config.SessionName : "(default)";
            int cap = config != null ? config.MaxPlayers : 8;
            EvaLog.Info($"Started local host session '{name}' for up to {cap} players.");
        }

        public void StartClient(string joinCode)
        {
            IsHosting = false;
            IsConnected = true;
            LastJoinCode = joinCode;
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
            LastJoinCode = null;
            EvaLog.Info("Disconnected local session.");
        }
    }
}
