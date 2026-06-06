namespace Evaverse.Networking.Runtime.Sessions
{
    public interface ISessionService
    {
        SessionBackend Backend { get; }
        bool IsHosting { get; }
        bool IsConnected { get; }
        bool IsBusy { get; }
        string JoinCode { get; }
        string StatusMessage { get; }
        void StartHost(SessionConfig config);
        void StartClient(string joinCode);
        void Disconnect();
        void NotifyTransportDisconnected();
    }
}
