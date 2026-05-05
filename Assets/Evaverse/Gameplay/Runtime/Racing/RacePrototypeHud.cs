using Evaverse.Gameplay.Runtime.Hoverboard;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Evaverse.Gameplay.Runtime.Racing
{
    public sealed class RacePrototypeHud : MonoBehaviour
    {
        [SerializeField] private RaceLapTracker tracker;
        [SerializeField] private HoverboardMotor hoverboard;
        [SerializeField] private bool showControls = true;

        private bool finishRecorded;
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;

        private void Update()
        {
            if (tracker != null && WasResetPressed())
            {
                tracker.ResetProgress();
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            RecordFinishIfNeeded();

            const float width = 330f;
            Rect panel = new Rect(18f, 18f, width, showControls ? 248f : 178f);
            GUI.Box(panel, GUIContent.none, panelStyle);

            GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));
            GUILayout.Label("Evaverse Prototype", titleStyle);
            GUILayout.Space(6f);

            string lapText = ResolveRaceLine();

            GUILayout.Label(lapText, labelStyle);
            GUILayout.Label($"Timer: {FormatTime(tracker != null ? tracker.ElapsedSeconds : 0f)}", labelStyle);
            GUILayout.Label($"Best: {FormatBestTime()}", labelStyle);

            string speedText = hoverboard == null || !hoverboard.enabled
                ? "Board: walk to the hoverboard and press E"
                : $"Board speed: {Mathf.Abs(hoverboard.CurrentSpeed):0.0} m/s";

            GUILayout.Label(speedText, labelStyle);

            if (showControls)
            {
                GUILayout.Space(8f);
                GUILayout.Label("WASD move/ride", labelStyle);
                GUILayout.Label("Shift sprint/drift  |  Space jump/boost", labelStyle);
                GUILayout.Label("E mount/dismount near board", labelStyle);
                GUILayout.Label("R reset race timer", labelStyle);
            }

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
                return $"Finished: {FormatTime(tracker.ElapsedSeconds)}";
            }

            if (!tracker.Started)
            {
                return "Race: enter the green start gate";
            }

            return $"Lap {tracker.CurrentLap}/{ResolveLapCount()}  |  Checkpoint {tracker.NextCheckpointIndex + 1}/{Mathf.Max(1, tracker.CheckpointCount)}";
        }

        private static bool WasResetPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.R);
#endif
        }

        private void RecordFinishIfNeeded()
        {
            if (tracker == null || !tracker.Finished)
            {
                finishRecorded = false;
                return;
            }

            if (finishRecorded)
            {
                return;
            }

            finishRecorded = true;

            string key = ResolveBestTimeKey();
            float previousBest = PlayerPrefs.GetFloat(key, 0f);
            float current = tracker.ElapsedSeconds;

            if (current <= 0f || (previousBest > 0f && current >= previousBest))
            {
                return;
            }

            PlayerPrefs.SetFloat(key, current);
            PlayerPrefs.Save();
        }

        private string FormatBestTime()
        {
            float best = PlayerPrefs.GetFloat(ResolveBestTimeKey(), 0f);
            return best > 0f ? FormatTime(best) : "--:--.--";
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
            panelTexture.SetPixel(0, 0, new Color(0.025f, 0.04f, 0.06f, 0.78f));
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
        }
    }
}
