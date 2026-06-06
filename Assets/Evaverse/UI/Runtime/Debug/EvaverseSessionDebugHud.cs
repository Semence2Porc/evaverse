using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Sessions;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.UI.Runtime.Debug
{
    public sealed class EvaverseSessionDebugHud : MonoBehaviour
    {
        [SerializeField] private SessionConfig sessionConfig;
        [SerializeField] private bool visible = true;

        private string joinInput = string.Empty;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;

        private void Start()
        {
            if (ServiceRegistry.TryResolve<ISessionService>(out var sessionService)
                && sessionService.Backend == SessionBackend.DirectNetcode
                && !string.IsNullOrWhiteSpace(sessionService.JoinCode))
            {
                joinInput = sessionService.JoinCode;
            }
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            EnsureStyles();

            if (!ServiceRegistry.TryResolve<ISessionService>(out var sessionService))
            {
                return;
            }

            const float width = 360f;
            Rect panel = new Rect(Screen.width - width - 18f, 18f, width, 250f);
            GUI.Box(panel, GUIContent.none);

            GUILayout.BeginArea(new Rect(panel.x + 14f, panel.y + 12f, panel.width - 28f, panel.height - 24f));
            GUILayout.Label("Evaverse Session", titleStyle);
            GUILayout.Space(4f);

            GUILayout.Label($"Backend: {sessionService.Backend}", labelStyle);
            GUILayout.Label($"Connected: {sessionService.IsConnected}", labelStyle);
            GUILayout.Label($"Hosting: {sessionService.IsHosting}", labelStyle);
            GUILayout.Label($"Status: {sessionService.StatusMessage}", labelStyle);

            if (!string.IsNullOrWhiteSpace(sessionService.JoinCode))
            {
                GUILayout.Label($"Join target: {sessionService.JoinCode}", labelStyle);
            }

            if (NetworkManager.Singleton != null)
            {
                GUILayout.Label($"Netcode listening: {NetworkManager.Singleton.IsListening}", labelStyle);
                GUILayout.Label($"Connected clients: {NetworkManager.Singleton.ConnectedClientsIds.Count}", labelStyle);
            }

            GUILayout.Space(8f);

            if (sessionService.Backend != SessionBackend.Local)
            {
                joinInput = GUILayout.TextField(joinInput, GUILayout.Height(24f));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Host", buttonStyle, GUILayout.Height(28f)))
                {
                    sessionService.StartHost(ResolveConfig());
                    if (!string.IsNullOrWhiteSpace(sessionService.JoinCode))
                    {
                        joinInput = sessionService.JoinCode;
                    }
                }

                if (GUILayout.Button("Join", buttonStyle, GUILayout.Height(28f)))
                {
                    sessionService.StartClient(joinInput);
                }

                if (GUILayout.Button("Disconnect", buttonStyle, GUILayout.Height(28f)))
                {
                    sessionService.Disconnect();
                    if (sessionService.Backend == SessionBackend.DirectNetcode
                        && !string.IsNullOrWhiteSpace(sessionService.JoinCode))
                    {
                        joinInput = sessionService.JoinCode;
                    }
                    else
                    {
                        joinInput = string.Empty;
                    }
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Local backend: use Playtest Overlay only.", labelStyle);
            }

            GUILayout.EndArea();
        }

        private SessionConfig ResolveConfig()
        {
            if (sessionConfig != null)
            {
                return sessionConfig;
            }

            sessionConfig = FindFirstObjectByType<SessionConfig>();
            return sessionConfig;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.72f, 0.2f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white },
                wordWrap = true
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12
            };
        }
    }
}
