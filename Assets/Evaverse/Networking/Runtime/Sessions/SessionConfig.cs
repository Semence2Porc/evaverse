using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class SessionConfig : MonoBehaviour
    {
        [SerializeField] private string sessionName = "Evaverse Session";
        [SerializeField] private int maxPlayers = 8;
        [SerializeField] private bool privateSession;

        public string SessionName => sessionName;
        public int MaxPlayers => Mathf.Max(1, maxPlayers);
        public bool PrivateSession => privateSession;
    }
}
