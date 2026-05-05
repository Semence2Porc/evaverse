namespace Evaverse.Networking.Runtime.Sessions
{
    public interface ISessionService
    {
        bool IsHosting { get; }
        bool IsConnected { get; }
        void StartHost(SessionConfig config);
        void StartClient(string joinCode);
        void Disconnect();
    }
}
