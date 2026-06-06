using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Netcode;
using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Gameplay.Runtime.View;
using Unity.Cinemachine.TargetTracking;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

namespace Evaverse.Networking.Editor
{
    /// <summary>
    /// Creates the networked player prefab at a fixed Resources path for <see cref="NetcodeBootstrap"/>.
    /// </summary>
    public static class EvaverseNetworkPrefabBaker
    {
        public const string PrefabPath = "Assets/Evaverse/Networking/Resources/Prefabs/EvaverseNetworkPlayer.prefab";

        [MenuItem("Evaverse/Networking/Bake Network Player Prefab")]
        public static void BakeFromMenu()
        {
            EnsurePrefab(true);
            Debug.Log($"Evaverse network player prefab saved to {PrefabPath}");
        }

        public static GameObject EnsurePrefab(bool forceRebuild = false)
        {
            EnsureFolder("Assets/Evaverse/Networking");
            EnsureFolder("Assets/Evaverse/Networking/Resources");
            EnsureFolder("Assets/Evaverse/Networking/Resources/Prefabs");

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existing != null && !forceRebuild)
            {
                return existing;
            }

            if (existing != null)
            {
                AssetDatabase.DeleteAsset(PrefabPath);
            }

            GameObject root = new GameObject("EvaverseNetworkPlayer");

            var netObj = root.AddComponent<NetworkObject>();
            netObj.DontDestroyWithOwner = false;

            var nt = root.AddComponent<NetworkTransform>();
            SerializedObject ntSerialized = new SerializedObject(nt);
            ntSerialized.FindProperty("AuthorityMode").enumValueIndex = (int)NetworkTransform.AuthorityModes.Owner;
            ntSerialized.ApplyModifiedPropertiesWithoutUndo();

            CharacterController cc = root.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.36f;
            cc.center = new Vector3(0f, 1f, 0f);

            AvatarMotor avatarMotor = root.AddComponent<AvatarMotor>();
            EvaverseNetOwnedPlayer netOwned = root.AddComponent<EvaverseNetOwnedPlayer>();
            RaceLapTracker lapTracker = root.AddComponent<RaceLapTracker>();

            Material bodyMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/World/evaverse_neon_cyan.mat");
            Material visorMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Materials/World/evaverse_neon_orange.mat");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Avatar Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(0.72f, 1f, 0.72f);
            if (bodyMaterial != null && body.TryGetComponent(out Renderer bodyRenderer))
            {
                bodyRenderer.sharedMaterial = bodyMaterial;
            }

            Object.DestroyImmediate(body.GetComponent<Collider>());

            GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visor.name = "Avatar Visor";
            visor.transform.SetParent(root.transform, false);
            visor.transform.localPosition = new Vector3(0f, 1.45f, 0.33f);
            visor.transform.localScale = new Vector3(0.5f, 0.12f, 0.08f);
            if (visorMaterial != null && visor.TryGetComponent(out Renderer visorRenderer))
            {
                visorRenderer.sharedMaterial = visorMaterial;
            }

            Object.DestroyImmediate(visor.GetComponent<Collider>());

            GameObject probe = new GameObject("Racing Trigger Probe");
            probe.transform.SetParent(root.transform, false);
            probe.transform.localPosition = new Vector3(0f, 1f, 0f);
            Rigidbody probeBody = probe.AddComponent<Rigidbody>();
            probeBody.isKinematic = true;
            probeBody.useGravity = false;
            CapsuleCollider probeCollider = probe.AddComponent<CapsuleCollider>();
            probeCollider.isTrigger = true;
            probeCollider.radius = 0.45f;
            probeCollider.height = 1.9f;
            probeCollider.direction = 1;
            probeCollider.center = Vector3.zero;

            GameObject pivot = new GameObject("Camera Pivot");
            pivot.transform.SetParent(root.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 1.68f, 0f);

            GameObject playerCamera = new GameObject("Main Camera");
            playerCamera.tag = "MainCamera";
            playerCamera.transform.SetParent(root.transform, false);
            Camera cam = playerCamera.AddComponent<Camera>();
            cam.fieldOfView = 58f;
            cam.nearClipPlane = 0.2f;
            cam.farClipPlane = 1800f;

            CinemachinePlayerRig rig = root.AddComponent<CinemachinePlayerRig>();

            GameObject hudObject = new GameObject("Race Prototype HUD");
            hudObject.transform.SetParent(root.transform, false);
            RacePrototypeHud hud = hudObject.AddComponent<RacePrototypeHud>();
            SerializedObject hudSo = new SerializedObject(hud);
            hudSo.FindProperty("tracker").objectReferenceValue = lapTracker;
            hudSo.FindProperty("showControls").boolValue = false;
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject avatarSo = new SerializedObject(avatarMotor);
            avatarSo.FindProperty("cameraPivot").objectReferenceValue = pivot.transform;
            avatarSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject rigSo = new SerializedObject(rig);
            rigSo.FindProperty("playerCamera").objectReferenceValue = cam;
            rigSo.FindProperty("cameraPivot").objectReferenceValue = pivot.transform;
            rigSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject ownerSo = new SerializedObject(netOwned);
            ownerSo.FindProperty("avatarMotor").objectReferenceValue = avatarMotor;
            ownerSo.FindProperty("characterController").objectReferenceValue = cc;
            ownerSo.FindProperty("lapTracker").objectReferenceValue = lapTracker;
            ownerSo.FindProperty("raceHud").objectReferenceValue = hud;
            ownerSo.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(path.Replace('\\', '/'));
            string leaf = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
