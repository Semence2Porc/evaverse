using Evaverse.Meta.Runtime.Progression;
using Evaverse.Meta.Runtime.Saves;
using UnityEngine;

namespace Evaverse.UI.Runtime.Meta
{
    public sealed class MetaProgressionHud : MonoBehaviour
    {
        [SerializeField] private bool visible = true;

        private PlayerProgressionState cachedState;
        private string statusMessage = string.Empty;
        private float statusUntil;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle statusStyle;

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

            EnsureStyles();
            if (cachedState == null)
            {
                Refresh();
            }

            const float width = 250f;
            const float height = 188f;
            Rect panel = new Rect(Screen.width - width - 20f, Screen.height - height - 20f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 14f, panel.y + 10f, panel.width - 28f, panel.height - 20f));
            GUILayout.Label("Pilot Log", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label($"Tickets: {cachedState.Tickets}", labelStyle);
            GUILayout.Label($"Races: {cachedState.RacesCompleted}", labelStyle);
            GUILayout.Label($"Board Lv {cachedState.HoverboardLevel}/{PlayerProgressionState.MaxBoardLevel}", labelStyle);
            GUILayout.Label($"Avatar Lv {cachedState.AvatarLevel}/{PlayerProgressionState.MaxAvatarLevel}", labelStyle);
            GUILayout.Space(6f);

            if (GUILayout.Button($"Upgrade Board ({PlayerProgressionState.BoardUpgradeCost})", buttonStyle, GUILayout.Height(26f)))
            {
                TryUpgradeBoard();
            }

            if (GUILayout.Button($"Upgrade Avatar ({PlayerProgressionState.AvatarUpgradeCost})", buttonStyle, GUILayout.Height(26f)))
            {
                TryUpgradeAvatar();
            }

            if (!string.IsNullOrEmpty(statusMessage) && Time.unscaledTime <= statusUntil)
            {
                GUILayout.Label(statusMessage, statusStyle);
            }

            GUILayout.EndArea();
        }

        private void TryUpgradeBoard()
        {
            Refresh();
            if (cachedState.TryUpgradeHoverboard())
            {
                PlayerProfileStore.Save(cachedState);
                statusMessage = "Board upgraded.";
            }
            else
            {
                statusMessage = "Need more tickets or already maxed.";
            }

            statusUntil = Time.unscaledTime + 2f;
            Refresh();
        }

        private void TryUpgradeAvatar()
        {
            Refresh();
            if (cachedState.TryUpgradeAvatar())
            {
                PlayerProfileStore.Save(cachedState);
                statusMessage = "Avatar upgraded.";
            }
            else
            {
                statusMessage = "Need more tickets or already maxed.";
            }

            statusUntil = Time.unscaledTime + 2f;
            Refresh();
        }

        private void Refresh()
        {
            cachedState = PlayerProfileStore.Load();
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

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11
            };

            statusStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(0.55f, 1f, 0.72f) }
            };
        }
    }
}
