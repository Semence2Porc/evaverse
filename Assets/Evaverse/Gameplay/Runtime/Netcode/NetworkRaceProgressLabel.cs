using Evaverse.Gameplay.Runtime.Racing;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class NetworkRaceProgressLabel : MonoBehaviour
    {
        [SerializeField] private RaceLapTracker tracker;
        [SerializeField] private TextMesh label;
        [SerializeField] private float labelHeight = 2.85f;

        private NetworkObject networkObject;
        private Camera viewCamera;

        public void Configure(RaceLapTracker raceTracker, TextMesh progressLabel)
        {
            tracker = raceTracker;
            label = progressLabel;
        }

        private void Awake()
        {
            networkObject = GetComponent<NetworkObject>();
        }

        private void LateUpdate()
        {
            if (label == null || tracker == null)
            {
                return;
            }

            if (networkObject != null && networkObject.IsOwner)
            {
                label.gameObject.SetActive(false);
                return;
            }

            string text = BuildLabelText();
            bool visible = !string.IsNullOrEmpty(text);
            label.gameObject.SetActive(visible);
            if (!visible)
            {
                return;
            }

            label.text = text;
            label.transform.position = transform.position + Vector3.up * labelHeight;
            FaceCamera(label.transform);
        }

        private string BuildLabelText()
        {
            if (tracker.CountdownActive)
            {
                return $"GO {Mathf.CeilToInt(tracker.CountdownRemaining)}";
            }

            if (tracker.Finished)
            {
                return $"FIN {FormatTime(tracker.ElapsedSeconds)}";
            }

            if (!tracker.Started)
            {
                return string.Empty;
            }

            int lapCount = tracker.Course != null ? tracker.Course.LapCount : 1;
            int checkpointCount = Mathf.Max(1, tracker.CheckpointCount);
            return $"L{tracker.CurrentLap}/{lapCount} CP{tracker.NextCheckpointIndex + 1}/{checkpointCount}";
        }

        private void FaceCamera(Transform target)
        {
            if (viewCamera == null)
            {
                viewCamera = Camera.main;
            }

            if (viewCamera == null)
            {
                return;
            }

            Vector3 forward = target.position - viewCamera.transform.position;
            if (forward.sqrMagnitude < 0.001f)
            {
                return;
            }

            target.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        private static string FormatTime(float seconds)
        {
            if (seconds <= 0f)
            {
                return "00:00";
            }

            int minutes = Mathf.FloorToInt(seconds / 60f);
            float remaining = seconds - minutes * 60f;
            return $"{minutes:00}:{remaining:00}";
        }
    }
}
