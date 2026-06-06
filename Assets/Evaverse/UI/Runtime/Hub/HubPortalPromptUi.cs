using Evaverse.World.Runtime.Hub;
using UnityEngine;

namespace Evaverse.UI.Runtime.Hub
{
    /// <summary>
    /// Shows a prompt when the local player enters a district portal trigger.
    /// </summary>
    public sealed class HubPortalPromptUi : MonoBehaviour
    {
        private HubDistrictPortal activePortal;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void OnEnable()
        {
            HubDistrictPortal.ProximityChanged += HandleProximityChanged;
        }

        private void OnDisable()
        {
            HubDistrictPortal.ProximityChanged -= HandleProximityChanged;
        }

        private void HandleProximityChanged(HubDistrictPortal portal, bool inside)
        {
            activePortal = inside ? portal : null;
        }

        private void OnGUI()
        {
            if (activePortal == null)
            {
                return;
            }

            EnsureStyles();

            const float width = 360f;
            const float height = 88f;
            Rect panel = new Rect((Screen.width - width) * 0.5f, Screen.height - height - 120f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));
            GUILayout.Label(activePortal.DistrictName, titleStyle);
            GUILayout.Label(activePortal.PromptText, bodyStyle);
            GUILayout.EndArea();
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

            panelStyle = new GUIStyle(GUI.skin.box) { normal = { background = panelTexture } };
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = Color.white },
                wordWrap = true
            };
        }
    }
}
