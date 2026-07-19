using Evaverse.Gameplay.Runtime.Hub;
using UnityEngine;

namespace Evaverse.UI.Runtime.Hub
{
    public sealed class DistrictPortalPromptHud : MonoBehaviour
    {
        [SerializeField] private float scanRadius = 8f;

        private GUIStyle labelStyle;
        private string prompt = string.Empty;

        private void Update()
        {
            prompt = string.Empty;
            Camera camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            DistrictPortal[] portals = FindObjectsByType<DistrictPortal>(FindObjectsSortMode.None);
            float best = scanRadius;
            DistrictPortal nearest = null;

            for (int i = 0; i < portals.Length; i++)
            {
                float distance = Vector3.Distance(camera.transform.position, portals[i].transform.position);
                if (distance >= best)
                {
                    continue;
                }

                best = distance;
                nearest = portals[i];
            }

            if (nearest != null)
            {
                prompt = $"F — Travel to {nearest.PortalLabel}";
            }
        }

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return;
            }

            EnsureStyles();
            Rect area = new Rect(Screen.width * 0.5f - 180f, Screen.height - 72f, 360f, 36f);
            GUI.Label(area, prompt, labelStyle);
        }

        private void EnsureStyles()
        {
            if (labelStyle != null)
            {
                return;
            }

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };
        }
    }
}
