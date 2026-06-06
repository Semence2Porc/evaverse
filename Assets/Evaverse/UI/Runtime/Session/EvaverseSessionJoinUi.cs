using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Sessions;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.UI.Runtime.Session
{
    public sealed class EvaverseSessionJoinUi : MonoBehaviour
    {
        [SerializeField] private SessionConfig sessionConfig;
        [SerializeField] private bool showDebugDetails;

        private string joinInput = string.Empty;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle labelStyle;
        private GUIStyle joinCodeStyle;
        private GUIStyle buttonStyle;
        private GUIStyle textFieldStyle;

        private void Start()
        {
            if (!ServiceRegistry.TryResolve<ISessionService>(out var sessionService))
            {
                return;
            }

            if (sessionService.Backend == SessionBackend.DirectNetcode
                && !string.IsNullOrWhiteSpace(sessionService.JoinCode))
            {
                joinInput = sessionService.JoinCode;
            }
        }

        private void OnGUI()
        {
            if (!ServiceRegistry.TryResolve<ISessionService>(out var sessionService)
                || sessionService.Backend == SessionBackend.Local)
            {
                return;
            }

            EnsureStyles();

            bool connected = sessionService.IsConnected;
            float panelHeight = connected ? 210f : 230f;
            if (showDebugDetails)
            {
                panelHeight += 56f;
            }

            const float width = 340f;
            Rect panel = new Rect(Screen.width - width - 20f, 20f, width, panelHeight);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 18f, panel.y + 14f, panel.width - 36f, panel.height - 28f));
            GUILayout.Label("Snowbound Nexus", titleStyle);
            GUILayout.Label(ResolveSubtitle(sessionService), subtitleStyle);
            GUILayout.Space(8f);

            if (connected)
            {
                DrawConnectedState(sessionService);
            }
            else
            {
                DrawDisconnectedState(sessionService);
            }

            if (showDebugDetails)
            {
                GUILayout.Space(6f);
                GUILayout.Label(ResolveDebugLine(sessionService), labelStyle);
            }

            GUILayout.EndArea();
        }

        private void DrawDisconnectedState(ISessionService sessionService)
        {
            GUILayout.Label("Host a session or join with a code.", labelStyle);
            GUILayout.Space(6f);

            GUILayout.Label("Join code / address", labelStyle);
            joinInput = GUILayout.TextField(joinInput, textFieldStyle, GUILayout.Height(30f));
            GUILayout.Space(8f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Host", buttonStyle, GUILayout.Height(34f)))
            {
                sessionService.StartHost(ResolveConfig());
                if (!string.IsNullOrWhiteSpace(sessionService.JoinCode))
                {
                    joinInput = sessionService.JoinCode;
                }
            }

            if (GUILayout.Button("Join", buttonStyle, GUILayout.Height(34f)))
            {
                sessionService.StartClient(joinInput);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawConnectedState(ISessionService sessionService)
        {
            string status = string.IsNullOrWhiteSpace(sessionService.StatusMessage)
                ? "Connected"
                : sessionService.StatusMessage;
            GUILayout.Label(status, labelStyle);

            if (!string.IsNullOrWhiteSpace(sessionService.JoinCode))
            {
                GUILayout.Space(6f);
                GUILayout.Label("Share this code", labelStyle);
                GUILayout.Label(sessionService.JoinCode, joinCodeStyle);
            }

            int playerCount = NetworkManager.Singleton != null
                ? NetworkManager.Singleton.ConnectedClientsIds.Count
                : 1;
            GUILayout.Label($"Players in session: {playerCount}", labelStyle);

            GUILayout.Space(10f);
            if (GUILayout.Button("Disconnect", buttonStyle, GUILayout.Height(32f)))
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
        }

        private static string ResolveSubtitle(ISessionService sessionService)
        {
            return sessionService.Backend switch
            {
                SessionBackend.MultiplayerRelay => "Online session · Relay",
                SessionBackend.DirectNetcode => "Online session · Direct",
                _ => "Online session"
            };
        }

        private static string ResolveDebugLine(ISessionService sessionService)
        {
            string listening = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening
                ? "listening"
                : "offline";

            return $"Debug · {sessionService.Backend} · hosting={sessionService.IsHosting} · netcode={listening}";
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
            if (panelStyle != null)
            {
                return;
            }

            Texture2D panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, new Color(0.03f, 0.05f, 0.08f, 0.9f));
            panelTexture.Apply();

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.72f, 0.82f, 0.9f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = Color.white },
                wordWrap = true
            };

            joinCodeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.72f, 0.2f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };

            textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
}
