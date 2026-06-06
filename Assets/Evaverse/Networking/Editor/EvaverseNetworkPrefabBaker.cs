using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Gameplay.Runtime.Netcode;
using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Gameplay.Runtime.View;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

namespace Evaverse.Networking.Editor
{
    public static class EvaverseNetworkPrefabBaker
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs";
        private const string PrefabPath = "Assets/_Project/Prefabs/NetworkPlayerPrototype.prefab";
        private const string MaterialFolder = "Assets/_Project/Materials/World";

        [MenuItem("Evaverse/Networking/Bake Network Player Prefab")]
        public static void BakeFromMenu()
        {
            Bake();
        }

        public static GameObject Bake()
        {
            EnsureFolder(PrefabFolder);

            Material cyan = LoadMaterial($"{MaterialFolder}/evaverse_neon_cyan.mat", new Color(0.04f, 0.93f, 1f));
            Material orange = LoadMaterial($"{MaterialFolder}/evaverse_neon_orange.mat", new Color(1f, 0.45f, 0.08f));
            Material metal = LoadMaterial($"{MaterialFolder}/evaverse_dark_metal.mat", new Color(0.055f, 0.064f, 0.075f));

            GameObject root = new GameObject("Network Player Prototype");

            CharacterController controller = root.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.36f;
            controller.center = new Vector3(0f, 1f, 0f);

            NetworkObject networkObject = root.AddComponent<NetworkObject>();
            NetworkTransform networkTransform = root.AddComponent<NetworkTransform>();
            networkTransform.SyncPositionX = true;
            networkTransform.SyncPositionY = true;
            networkTransform.SyncPositionZ = true;
            networkTransform.SyncRotAngleY = true;

            root.AddComponent<AvatarMotor>();
            root.AddComponent<HoverboardMountController>();
            root.AddComponent<RaceLapTracker>();
            root.AddComponent<NetworkPlayerMountSync>();
            root.AddComponent<RaceLapTrackerNetworkSync>();
            root.AddComponent<EvaverseNetOwnedPlayer>();

            GameObject racingProbe = new GameObject("Racing Trigger Probe");
            racingProbe.transform.SetParent(root.transform, false);
            racingProbe.transform.localPosition = new Vector3(0f, 1f, 0f);
            Rigidbody probeBody = racingProbe.AddComponent<Rigidbody>();
            probeBody.isKinematic = true;
            probeBody.useGravity = false;
            CapsuleCollider probeCollider = racingProbe.AddComponent<CapsuleCollider>();
            probeCollider.isTrigger = true;
            probeCollider.radius = 0.45f;
            probeCollider.height = 1.9f;
            probeCollider.direction = 1;

            GameObject pivot = new GameObject("Camera Pivot");
            pivot.transform.SetParent(root.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 1.68f, 0f);

            GameObject playerCamera = new GameObject("Main Camera");
            playerCamera.tag = "MainCamera";
            playerCamera.transform.SetParent(root.transform, false);
            Camera camera = playerCamera.AddComponent<Camera>();
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.2f;
            camera.farClipPlane = 1800f;
            playerCamera.AddComponent<AudioListener>();

            SerializedObject avatarObject = new SerializedObject(root.GetComponent<AvatarMotor>());
            avatarObject.FindProperty("cameraPivot").objectReferenceValue = pivot.transform;
            avatarObject.ApplyModifiedPropertiesWithoutUndo();

            root.AddComponent<CinemachinePlayerRig>();
            root.AddComponent<RacePrototypeHud>();
            root.AddComponent<RaceCheckpointGuide>();
            root.AddComponent<NetworkRaceProgressLabel>();
            root.AddComponent<NetworkPlayerDisplayName>();
            root.AddComponent<RaceFinishReporter>();

            CreateVisual("avatar-capsule-visual", PrimitiveType.Capsule, root.transform, new Vector3(0f, 1f, 0f), new Vector3(0.72f, 1f, 0.72f), cyan);
            CreateVisual("avatar-visor", PrimitiveType.Cube, root.transform, new Vector3(0f, 1.45f, 0.33f), new Vector3(0.5f, 0.12f, 0.08f), orange);
            CreateVisual("avatar-backpack", PrimitiveType.Cube, root.transform, new Vector3(0f, 1.2f, -0.28f), new Vector3(0.42f, 0.5f, 0.18f), metal);

            CreatePersonalHoverboard(root.transform, metal, orange);
            CreateCheckpointArrow(root.transform, orange);
            CreateRaceProgressLabel(root.transform);
            CreatePlayerNameLabel(root.transform);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            RegisterNetworkPrefab(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Network player prefab baked at {PrefabPath}.");
            return prefab;
        }

        private static void CreatePlayerNameLabel(Transform parent)
        {
            GameObject labelObject = new GameObject("Player Name Label");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = new Vector3(0f, 2.15f, 0f);
            labelObject.SetActive(false);

            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = string.Empty;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.1f;
            textMesh.fontSize = 42;
            textMesh.color = Color.white;
        }

        private static void CreateRaceProgressLabel(Transform parent)
        {
            GameObject labelObject = new GameObject("Race Progress Label");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = new Vector3(0f, 2.85f, 0f);
            labelObject.SetActive(false);

            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = string.Empty;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.12f;
            textMesh.fontSize = 48;
            textMesh.color = new Color(0.2f, 0.92f, 1f);
        }

        private static void CreateCheckpointArrow(Transform parent, Material material)
        {
            GameObject arrow = new GameObject("Checkpoint Arrow");
            arrow.transform.SetParent(parent, false);
            arrow.transform.localPosition = new Vector3(0f, 2.6f, 0f);
            arrow.SetActive(false);

            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mesh.name = "checkpoint-arrow-mesh";
            mesh.transform.SetParent(arrow.transform, false);
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f);
            if (mesh.TryGetComponent(out Collider collider))
            {
                Object.DestroyImmediate(collider);
            }

            if (material != null && mesh.TryGetComponent(out Renderer renderer))
            {
                renderer.sharedMaterial = material;
            }
        }

        private static void CreatePersonalHoverboard(Transform parent, Material deckMaterial, Material trimMaterial)
        {
            GameObject board = new GameObject("Personal Hoverboard");
            board.transform.SetParent(parent, false);
            board.transform.localPosition = new Vector3(2.5f, 0f, -1.2f);
            board.transform.localRotation = Quaternion.Euler(0f, -18f, 0f);

            Rigidbody body = board.AddComponent<Rigidbody>();
            body.mass = 24f;
            body.useGravity = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            BoxCollider boardCollider = board.AddComponent<BoxCollider>();
            boardCollider.center = new Vector3(0f, 0.18f, 0f);
            boardCollider.size = new Vector3(2.8f, 0.38f, 6.2f);

            HoverboardMotor boardMotor = board.AddComponent<HoverboardMotor>();
            boardMotor.enabled = false;
            board.AddComponent<HoverboardMount>();

            CreateVisual("hoverboard-deck-visual", PrimitiveType.Cube, board.transform, new Vector3(0f, 0.2f, 0f), new Vector3(2.8f, 0.22f, 6.2f), deckMaterial);
            CreateVisual("hoverboard-neon-trim", PrimitiveType.Cube, board.transform, new Vector3(0f, 0.36f, 0f), new Vector3(3.05f, 0.08f, 6.5f), trimMaterial);

            GameObject groundProbe = new GameObject("Ground Probe");
            groundProbe.transform.SetParent(board.transform, false);
            groundProbe.transform.localPosition = new Vector3(0f, -0.45f, 0f);

            GameObject riderSocket = new GameObject("Rider Socket");
            riderSocket.transform.SetParent(board.transform, false);
            riderSocket.transform.localPosition = new Vector3(0f, 1.15f, -0.2f);

            GameObject dismountPoint = new GameObject("Dismount Point");
            dismountPoint.transform.SetParent(board.transform, false);
            dismountPoint.transform.localPosition = new Vector3(3.2f, 0.25f, 0f);

            SerializedObject boardMotorObject = new SerializedObject(boardMotor);
            boardMotorObject.FindProperty("groundProbe").objectReferenceValue = groundProbe.transform;
            boardMotorObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject mountObject = new SerializedObject(board.GetComponent<HoverboardMount>());
            mountObject.FindProperty("riderSocket").objectReferenceValue = riderSocket.transform;
            mountObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RegisterNetworkPrefab(GameObject prefab)
        {
            const string assetPath = "Assets/DefaultNetworkPrefabs.asset";
            ScriptableObject defaultPrefabs = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (defaultPrefabs == null)
            {
                Debug.LogWarning("DefaultNetworkPrefabs asset not found; player prefab was baked but not registered.");
                return;
            }

            SerializedObject serialized = new SerializedObject(defaultPrefabs);
            SerializedProperty list = serialized.FindProperty("List");
            if (list == null || !list.isArray)
            {
                Debug.LogWarning("DefaultNetworkPrefabs asset has no List property.");
                return;
            }

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                SerializedProperty prefabProperty = element.FindPropertyRelative("Prefab");
                if (prefabProperty != null && prefabProperty.objectReferenceValue == prefab)
                {
                    return;
                }
            }

            int index = list.arraySize;
            list.InsertArrayElementAtIndex(index);
            SerializedProperty added = list.GetArrayElementAtIndex(index);
            SerializedProperty addedPrefab = added.FindPropertyRelative("Prefab");
            if (addedPrefab != null)
            {
                addedPrefab.objectReferenceValue = prefab;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(defaultPrefabs);
        }

        private static void CreateVisual(string name, PrimitiveType primitive, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
        {
            GameObject visual = GameObject.CreatePrimitive(primitive);
            visual.name = name;
            visual.transform.SetParent(parent, false);
            visual.transform.localPosition = localPosition;
            visual.transform.localScale = scale;
            if (visual.TryGetComponent(out Collider collider))
            {
                Object.DestroyImmediate(collider);
            }

            if (material != null && visual.TryGetComponent(out Renderer renderer))
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material LoadMaterial(string path, Color fallbackColor)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", fallbackColor);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", fallbackColor);
            }

            return material;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
