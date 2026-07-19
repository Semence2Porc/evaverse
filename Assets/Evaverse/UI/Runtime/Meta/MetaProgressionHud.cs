using Evaverse.Meta.Runtime.Progression;
using Evaverse.Meta.Runtime.Saves;
using UnityEngine;

namespace Evaverse.UI.Runtime.Meta
{
    public sealed class MetaProgressionHud : MonoBehaviour
    {
        [SerializeField] private bool visible = true;

        private PlayerProgressionState cachedState;
        private float nextRefreshAt;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;

        private void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            if (Time.unscaledTime >= nextRefreshAt)
            {
                Refresh();
            }

            EnsureStyles();

            const float width = 220f;
            const float height = 118f;
            Rect panel = new Rect(Screen.width - width - 20f, Screen.height - height - 20f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 14f, panel.y + 10f, panel.width - 28f, panel.height - 20f));
            GUILayout.Label("Pilot Log", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label($"Tickets: {cachedState?.Tickets ?? 0}", labelStyle);
            GUILayout.Label($"Races: {cachedState?.RacesCompleted ?? 0}", labelStyle);
            GUILayout.Label($"Board Lv {cachedState?.HoverboardLevel ?? 1} · Avatar Lv {cachedState?.AvatarLevel ?? 1}", labelStyle);
            GUILayout.EndArea();
        }

        private void Refresh()
        {
            cachedState = PlayerProfileStore.Load();
            nextRefreshAt = Time.unscaledTime + 1.5f;
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

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
        }
    }
}
