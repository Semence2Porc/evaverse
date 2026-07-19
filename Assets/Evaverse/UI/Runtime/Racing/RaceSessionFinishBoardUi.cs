using System.Collections.Generic;
using Evaverse.Gameplay.Runtime.Netcode;
using Evaverse.Gameplay.Runtime.Racing;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.UI.Runtime.Racing
{
    public sealed class RaceSessionFinishBoardUi : MonoBehaviour
    {
        [SerializeField] private bool visible = true;

        private NetworkRaceSessionTracker sessionTracker;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle rowStyle;
        private GUIStyle waitingStyle;

        private void OnEnable()
        {
            sessionTracker = NetworkRaceSessionTracker.Instance ?? FindFirstObjectByType<NetworkRaceSessionTracker>();
            if (sessionTracker != null)
            {
                sessionTracker.Finishes.OnListChanged += HandleFinishesChanged;
            }
        }

        private void OnDisable()
        {
            if (sessionTracker != null)
            {
                sessionTracker.Finishes.OnListChanged -= HandleFinishesChanged;
            }
        }

        private void HandleFinishesChanged(NetworkListEvent<RaceFinishEntry> changeEvent)
        {
        }

        private void OnGUI()
        {
            if (!visible || NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                return;
            }

            if (sessionTracker == null)
            {
                sessionTracker = NetworkRaceSessionTracker.Instance ?? FindFirstObjectByType<NetworkRaceSessionTracker>();
                if (sessionTracker == null)
                {
                    return;
                }
            }

            EnsureStyles();
            List<RaceFinishEntry> finishes = sessionTracker.GetSortedFinishes();

            const float width = 360f;
            float height = finishes.Count == 0 ? 118f : 88f + finishes.Count * 24f;
            Rect panel = new Rect(20f, Screen.height - height - 20f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));
            GUILayout.Label("Session Finish Board", titleStyle);
            GUILayout.Space(6f);

            if (finishes.Count == 0)
            {
                GUILayout.Label("Hit the green gate to start a synced race.", waitingStyle);
                GUILayout.Label("Waiting for the first finisher...", waitingStyle);
                GUILayout.EndArea();
                return;
            }

            for (int i = 0; i < finishes.Count; i++)
            {
                RaceFinishEntry entry = finishes[i];
                string row = $"{entry.Place}. {entry.DisplayName}  —  {FormatTime(entry.FinishSeconds)}";
                GUILayout.Label(row, rowStyle);
            }

            GUILayout.EndArea();
        }

        private static string FormatTime(float seconds)
        {
            if (seconds <= 0f)
            {
                return "00:00.00";
            }

            int minutes = Mathf.FloorToInt(seconds / 60f);
            float remaining = seconds - minutes * 60f;
            return $"{minutes:00}:{remaining:00.00}";
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            Texture2D panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, new Color(0.02f, 0.05f, 0.08f, 0.88f));
            panelTexture.Apply();

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.72f, 0.2f) }
            };

            rowStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = Color.white }
            };

            waitingStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(0.72f, 0.82f, 0.9f) }
            };
        }
    }
}
