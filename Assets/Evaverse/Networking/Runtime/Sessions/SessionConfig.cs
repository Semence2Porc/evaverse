using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class SessionConfig : MonoBehaviour
    {
        [SerializeField] private string sessionName = "Evaverse Session";
        [SerializeField] private int maxPlayers = 8;
        [SerializeField] private bool privateSession;

        [Header("Netcode (UDP listen / client dial)")]
        [SerializeField] [Range(1, 65534)] private int listenPort = 7777;
        [SerializeField] private string clientDefaultAddress = "127.0.0.1";

        public string SessionName => sessionName;
        public int MaxPlayers => Mathf.Max(1, maxPlayers);
        public bool PrivateSession => privateSession;
        public ushort ListenPort => (ushort)Mathf.Clamp(listenPort, 1, 65534);
        public string ClientDefaultAddress =>
            string.IsNullOrWhiteSpace(clientDefaultAddress) ? "127.0.0.1" : clientDefaultAddress.Trim();
    }
}
