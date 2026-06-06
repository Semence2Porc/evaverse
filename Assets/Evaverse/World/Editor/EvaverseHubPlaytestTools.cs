using System;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Networking.Runtime.Netcode;
using Evaverse.Networking.Runtime.Sessions;
using Evaverse.UI.Runtime.Debug;
using Unity.Netcode;
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

            bool isNetworkHub = GameObject.Find("_EvaverseNetworking") != null;
            bool valid = true;

            valid &= RequireObject("Prototype Hoverboard");
            valid &= RequireObject("race-start-gate");
            valid &= RequireObject("race-start-trigger");
            valid &= RequireObject("race-checkpoint-00");

            RaceCourseDefinition course = UnityEngine.Object.FindFirstObjectByType<RaceCourseDefinition>();
            RaceStartGate startGate = UnityEngine.Object.FindFirstObjectByType<RaceStartGate>();
            valid &= RequireComponent(course, "RaceCourseDefinition");
            valid &= RequireComponent(startGate, "RaceStartGate");

            if (isNetworkHub)
            {
                valid &= RequireObject("_EvaverseNetworking");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<NetworkManager>(), "NetworkManager");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<NetcodeBootstrap>(), "NetcodeBootstrap");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<SessionBootstrap>(), "SessionBootstrap");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<EvaverseSessionDebugHud>(), "EvaverseSessionDebugHud");
            }
            else
            {
                valid &= RequireObject("Local Player Prototype");
                valid &= RequireObject("Race Prototype HUD");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<RacePrototypeHud>(), "RacePrototypeHud");
                valid &= RequireComponent(UnityEngine.Object.FindFirstObjectByType<HoverboardMountController>(), "HoverboardMountController");
            }

            if (course != null && course.CheckpointCount <= 0)
            {
                valid = false;
                Debug.LogError("Hub validation failed: RaceCourseDefinition has no checkpoints.");
            }

            if (!valid)
            {
                throw new InvalidOperationException("Evaverse hub validation failed. See Unity log for details.");
            }

            string hubMode = isNetworkHub ? "network" : "local";
            Debug.Log($"Evaverse hub validation passed ({hubMode}): {course.CheckpointCount} checkpoints, start gate and core objects found.");
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
