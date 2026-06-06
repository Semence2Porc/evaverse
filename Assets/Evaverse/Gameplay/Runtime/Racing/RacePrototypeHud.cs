using Evaverse.Gameplay.Runtime.Hoverboard;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public sealed class RacePrototypeHud : MonoBehaviour
    {
        [SerializeField] private RaceLapTracker tracker;
        [SerializeField] private HoverboardMotor hoverboard;
        [SerializeField] private bool showControls = true;

        private bool finishRecorded;
        private bool showedNewBest;
        private float finishBestBeforeRun;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle finishTitleStyle;
        private GUIStyle finishTimeStyle;
        private GUIStyle finishAccentStyle;

        public void Configure(RaceLapTracker raceTracker, HoverboardMotor boardMotor)
        {
            tracker = raceTracker;
            hoverboard = boardMotor;
        }

        private void Update()
        {
            if (tracker == null)
            {
                return;
            }

            if (!tracker.Started && !tracker.Finished)
            {
                finishRecorded = false;
                showedNewBest = false;
                finishBestBeforeRun = 0f;
            }

            if (tracker.Started && !tracker.Finished && finishBestBeforeRun <= 0f)
            {
                finishBestBeforeRun = PlayerPrefs.GetFloat(ResolveBestTimeKey(), 0f);
            }

            if (WasResetPressed())
            {
                tracker.ResetProgress();
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            RecordFinishIfNeeded();
            DrawRacePanel();

            if (tracker != null && tracker.Finished)
            {
                DrawFinishSummary();
            }
        }

        private void DrawRacePanel()
        {
            const float width = 330f;
            Rect panel = new Rect(18f, 18f, width, showControls ? 260f : 190f);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));
            GUILayout.Label("EVA Grand Prix", titleStyle);
            GUILayout.Space(6f);

            GUILayout.Label(ResolveRaceLine(), labelStyle);
            GUILayout.Label($"Timer: {FormatTime(tracker != null ? tracker.ElapsedSeconds : 0f)}", labelStyle);
            GUILayout.Label(ResolveBestLine(), labelStyle);
            GUILayout.Label(ResolveBestDeltaLine(), labelStyle);

            string speedText = hoverboard == null || !hoverboard.enabled
                ? "Board: press E near your hoverboard"
                : $"Board speed: {Mathf.Abs(hoverboard.CurrentSpeed):0.0} m/s";

            GUILayout.Label(speedText, labelStyle);

            if (showControls)
            {
                GUILayout.Space(8f);
                GUILayout.Label("WASD move/ride", labelStyle);
                GUILayout.Label("Shift sprint/drift  |  Space jump/boost", labelStyle);
                GUILayout.Label("E / gamepad X — mount/dismount", labelStyle);
                GUILayout.Label("R reset race", labelStyle);
            }

            GUILayout.EndArea();
        }

        private void DrawFinishSummary()
        {
            const float width = 420f;
            const float height = 220f;
            Rect panel = new Rect((Screen.width - width) * 0.5f, Screen.height * 0.28f, width, height);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 20f, panel.y + 16f, panel.width - 40f, panel.height - 32f));
            GUILayout.Label("Run Complete", finishTitleStyle);
            GUILayout.Space(8f);
            GUILayout.Label($"Finish time: {FormatTime(tracker.ElapsedSeconds)}", finishTimeStyle);

            float best = PlayerPrefs.GetFloat(ResolveBestTimeKey(), 0f);
            GUILayout.Label($"Personal best: {(best > 0f ? FormatTime(best) : "--:--.--")}", labelStyle);

            if (showedNewBest)
            {
                GUILayout.Space(6f);
                GUILayout.Label("NEW PERSONAL BEST!", finishAccentStyle);
            }
            else if (best > 0f)
            {
                float delta = tracker.ElapsedSeconds - best;
                string deltaText = delta > 0f
                    ? $"+{delta:0.00}s vs best"
                    : $"{delta:0.00}s vs best";
                GUILayout.Label(deltaText, labelStyle);
            }

            GUILayout.Space(10f);
            GUILayout.Label("Press R to run again", labelStyle);
            GUILayout.EndArea();
        }

        private string ResolveRaceLine()
        {
            if (tracker == null)
            {
                return "Race: no tracker";
            }

            if (tracker.CountdownActive)
            {
                return $"Race starts in {Mathf.CeilToInt(tracker.CountdownRemaining)}";
            }

            if (tracker.Finished)
            {
                return "Race finished";
            }

            if (!tracker.Started)
            {
                return "Enter the green start gate";
            }

            return $"Lap {tracker.CurrentLap}/{ResolveLapCount()}  |  Checkpoint {tracker.NextCheckpointIndex + 1}/{Mathf.Max(1, tracker.CheckpointCount)}";
        }

        private string ResolveBestLine()
        {
            float best = PlayerPrefs.GetFloat(ResolveBestTimeKey(), 0f);
            return $"Best: {(best > 0f ? FormatTime(best) : "--:--.--")}";
        }

        private string ResolveBestDeltaLine()
        {
            if (tracker == null || !tracker.Started || tracker.Finished)
            {
                return string.Empty;
            }

            float best = PlayerPrefs.GetFloat(ResolveBestTimeKey(), 0f);
            if (best <= 0f)
            {
                return "Delta: first timed run";
            }

            float delta = tracker.ElapsedSeconds - best;
            return delta <= 0f
                ? $"Delta: {delta:0.00}s (ahead)"
                : $"Delta: +{delta:0.00}s (behind best)";
        }

        private static bool WasResetPressed()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                return true;
            }

            return Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame;
        }

        private void RecordFinishIfNeeded()
        {
            if (tracker == null || !tracker.Finished)
            {
                return;
            }

            if (finishRecorded)
            {
                return;
            }

            finishRecorded = true;

            string key = ResolveBestTimeKey();
            float previousBest = finishBestBeforeRun > 0f ? finishBestBeforeRun : PlayerPrefs.GetFloat(key, 0f);
            float current = tracker.ElapsedSeconds;
            showedNewBest = current > 0f && (previousBest <= 0f || current < previousBest);

            if (!showedNewBest)
            {
                return;
            }

            PlayerPrefs.SetFloat(key, current);
            PlayerPrefs.Save();
        }

        private string ResolveBestTimeKey()
        {
            string courseId = tracker != null && tracker.Course != null ? tracker.Course.CourseId : "unknown-course";
            return $"Evaverse.RacePrototype.BestTime.{courseId}";
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

        private int ResolveLapCount()
        {
            return tracker != null && tracker.Course != null ? tracker.Course.LapCount : 1;
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            Texture2D panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, new Color(0.025f, 0.04f, 0.06f, 0.82f));
            panelTexture.Apply();

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture },
                padding = new RectOffset(14, 14, 12, 12)
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = Color.white }
            };

            finishTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.2f, 0.92f, 1f) }
            };

            finishTimeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            finishAccentStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 0.72f, 0.2f) }
            };
        }
    }
}
