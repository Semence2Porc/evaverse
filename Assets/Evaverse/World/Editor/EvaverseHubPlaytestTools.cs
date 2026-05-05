using System;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Gameplay.Runtime.Racing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Evaverse.World.Editor
{
    public static class EvaverseHubPlaytestTools
    {
        private const string HubScenePath = "Assets/_Project/Scenes/Hub.unity";

        [MenuItem("Evaverse/Playtest/Open Hub")]
        public static void OpenHub()
        {
            EditorSceneManager.OpenScene(HubScenePath);
        }

        [MenuItem("Evaverse/Playtest/Open Hub And Play")]
        public static void OpenHubAndPlay()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            OpenHub();
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("Evaverse/Playtest/Validate Hub Scene")]
        public static void ValidateHubSceneFromMenu()
        {
            ValidateHubScene();
        }

        public static void ValidateHubSceneFromCommandLine()
        {
            ValidateHubScene();
        }

        private static void ValidateHubScene()
        {
            EditorSceneManager.OpenScene(HubScenePath);

            bool valid = true;
            valid &= RequireObject("Local Player Prototype");
            valid &= RequireObject("Prototype Hoverboard");
            valid &= RequireObject("Race Prototype HUD");
            valid &= RequireObject("race-start-gate");
            valid &= RequireObject("race-start-trigger");
            valid &= RequireObject("race-checkpoint-00");

            RaceCourseDefinition course = UnityEngine.Object.FindFirstObjectByType<RaceCourseDefinition>();
            RaceLapTracker[] trackers = UnityEngine.Object.FindObjectsByType<RaceLapTracker>(FindObjectsSortMode.None);
            RacePrototypeHud hud = UnityEngine.Object.FindFirstObjectByType<RacePrototypeHud>();
            HoverboardMountController mountController = UnityEngine.Object.FindFirstObjectByType<HoverboardMountController>();
            RaceStartGate startGate = UnityEngine.Object.FindFirstObjectByType<RaceStartGate>();

            valid &= RequireComponent(course, "RaceCourseDefinition");
            valid &= RequireComponent(hud, "RacePrototypeHud");
            valid &= RequireComponent(mountController, "HoverboardMountController");
            valid &= RequireComponent(startGate, "RaceStartGate");

            if (course != null && course.CheckpointCount <= 0)
            {
                valid = false;
                Debug.LogError("Hub validation failed: RaceCourseDefinition has no checkpoints.");
            }

            if (trackers.Length < 2)
            {
                valid = false;
                Debug.LogError($"Hub validation failed: expected at least 2 RaceLapTracker components, found {trackers.Length}.");
            }

            if (!valid)
            {
                throw new InvalidOperationException("Evaverse hub validation failed. See Unity log for details.");
            }

            Debug.Log($"Evaverse hub validation passed: {course.CheckpointCount} checkpoints, {trackers.Length} race trackers, start gate and HUD found.");
        }

        private static bool RequireObject(string objectName)
        {
            if (GameObject.Find(objectName) != null)
            {
                return true;
            }

            Debug.LogError($"Hub validation failed: missing GameObject '{objectName}'.");
            return false;
        }

        private static bool RequireComponent(UnityEngine.Object component, string label)
        {
            if (component != null)
            {
                return true;
            }

            Debug.LogError($"Hub validation failed: missing component '{label}'.");
            return false;
        }
    }
}
