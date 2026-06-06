using Evaverse.Core.Runtime.App;
using Evaverse.Meta.Runtime.Progression;
using Evaverse.Meta.Runtime.Saves;
using Evaverse.Networking.Runtime.Sessions;
using UnityEngine;

namespace Evaverse.UI.Runtime.Meta
{
    /// <summary>
    /// Lightweight IMGUI strip showing local progression from <see cref="PlayerProfileStore"/>.
    /// </summary>
    public sealed class PlayerMetaHud : MonoBehaviour
    {
        [SerializeField] private bool visible = true;

        private string displayNameInput = "Racer";
        private GUIStyle panelStyle;
        private GUIStyle labelStyle;
        private GUIStyle valueStyle;
        private GUIStyle textFieldStyle;

        private void Start()
        {
            displayNameInput = PlayerProfileStore.LoadDisplayName();
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            EnsureStyles();

            var profile = PlayerProfileStore.Load();
            bool showLocalNameField = ShouldShowLocalNameField();

            const float width = 220f;
            float height = showLocalNameField ? 132f : 92f;
            Rect panel = new Rect(18f, Screen.height - height - 18f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 12f, panel.y + 10f, panel.width - 24f, panel.height - 20f));
            GUILayout.Label("Profile", labelStyle);
            if (showLocalNameField)
            {
                GUILayout.Label("Name", labelStyle);
                string updated = GUILayout.TextField(displayNameInput, textFieldStyle, GUILayout.Height(24f));
                if (!string.Equals(updated, displayNameInput, System.StringComparison.Ordinal))
                {
                    displayNameInput = updated;
                    PlayerProfileStore.SaveDisplayName(PlayerProgressionState.SanitizeDisplayName(displayNameInput));
                    displayNameInput = PlayerProfileStore.LoadDisplayName();
                }
            }

            GUILayout.Label($"Tickets  {profile.Tickets}", valueStyle);
            GUILayout.Label($"Level    {profile.AvatarLevel}", valueStyle);
            GUILayout.Label($"Races    {profile.RacesCompleted}", valueStyle);
            GUILayout.EndArea();
        }

        private static bool ShouldShowLocalNameField()
        {
            if (!ServiceRegistry.TryResolve<ISessionService>(out var session))
            {
                return true;
            }

            return session.Backend == SessionBackend.Local || !session.IsConnected;
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            Texture2D panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, new Color(0.03f, 0.05f, 0.08f, 0.82f));
            panelTexture.Apply();

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.55f, 0.88f, 1f) }
            };

            valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = Color.white }
            };

            textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 12
            };
        }
    }
}
