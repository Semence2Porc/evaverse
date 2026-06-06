using Evaverse.Core.Runtime.App;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    /// <summary>
    /// Minimal IMGUI controls to boot a listen-server host or join a local client (prototype only).
    /// Disabled by default — enable when using <see cref="SessionBackend.Netcode"/>.
    /// </summary>
    public sealed class EvaverseSessionDebugHud : MonoBehaviour
    {
        [SerializeField] private string joinTarget;
        [SerializeField] private bool autoFillJoinFromLastCode = true;

        private void OnGUI()
        {
            if (!Application.isPlaying || !enabled)
            {
                return;
            }

            if (!ServiceRegistry.TryResolve(out ISessionService session))
            {
                return;
            }

            bool isRelay = session is MultiplayerRelaySessionService;
            string hostLabel = isRelay ? "Host (relay)" : "Host (listen)";
            string joinLabel = isRelay ? "Join (code)" : "Join (ip:port)";
            if (!isRelay && string.IsNullOrWhiteSpace(joinTarget))
            {
                SessionConfig cfg = FindSessionConfig();
                joinTarget = cfg != null ? $"{cfg.ClientDefaultAddress}:{cfg.ListenPort}" : "127.0.0.1:7777";
            }

            const float w = 300f;
            Rect r = new Rect(18f, Screen.height - 220f, w, 196f);
            GUILayout.BeginArea(r);
            GUILayout.Label("Session (debug)", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.Label(session.IsConnected
                ? session.IsHosting ? "Status: hosting" : "Status: connected"
                : "Status: disconnected");
            GUILayout.Space(4f);

            if (GUILayout.Button(hostLabel))
            {
                SessionConfig settings = FindSessionConfig();
                session.StartHost(settings);
            }

            if (!string.IsNullOrWhiteSpace(session.LastJoinCode))
            {
                GUILayout.Space(4f);
                GUILayout.Label($"Join code: {session.LastJoinCode}");
                if (GUILayout.Button("Copy join code"))
                {
                    GUIUtility.systemCopyBuffer = session.LastJoinCode;
                    EvaLog.Info($"Copied join code '{session.LastJoinCode}' to clipboard.", this);
                }

                if (autoFillJoinFromLastCode && string.IsNullOrWhiteSpace(joinTarget))
                {
                    joinTarget = session.LastJoinCode;
                }
            }

            joinTarget = GUILayout.TextField(joinTarget, GUILayout.Height(24f));

            if (GUILayout.Button(joinLabel))
            {
                session.StartClient(joinTarget);
            }

            if (GUILayout.Button("Disconnect"))
            {
                session.Disconnect();
            }

            GUILayout.EndArea();
        }

        private SessionConfig FindSessionConfig()
        {
            if (TryGetComponent(out SessionConfig cfg))
            {
                return cfg;
            }

            return FindFirstObjectByType<SessionConfig>();
        }
    }
}
