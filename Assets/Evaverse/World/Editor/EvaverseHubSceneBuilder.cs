using System;
using System.Collections.Generic;
using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Gameplay.Runtime.View;
using Evaverse.World.Runtime.Authoring;
using Evaverse.World.Runtime.Definitions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evaverse.World.Editor
{
    public static class EvaverseHubSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Hub.unity";
        private const string MapDefinitionPath = "Assets/_Project/Settings/World/EvaverseRevivalHub.asset";
        private const string MaterialFolder = "Assets/_Project/Materials/World";
        private const string MeshFolder = "Assets/_Project/Art/Generated/World";
        private const string SeedPath = "Assets/_Project/MapSeeds/EvaverseHubSeed.json";

        private static readonly Vector3 PlazaCenter = Vector3.zero;

        [MenuItem("Evaverse/World/Build Revival Hub Scene")]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static void BuildFromCommandLine()
        {
            Build();
        }

        private static void Build()
        {
            EnsureProjectFolders();

            MaterialLibrary materials = MaterialLibrary.Create(MaterialFolder);
            Mesh hexMesh = CreateOrUpdateMesh($"{MeshFolder}/hex_prism.asset", CreateHexPrismMesh);
            Mesh spireMesh = CreateOrUpdateMesh($"{MeshFolder}/ice_spire.asset", CreateSpireMesh);
            WorldMapDefinition mapDefinition = CreateOrUpdateMapDefinition();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject root = new GameObject("_EvaverseRevivalHub");
            GameObject worldRoot = new GameObject("_World");
            worldRoot.transform.SetParent(root.transform, false);

            ConfigureWorldComponents(worldRoot, mapDefinition);
            BuildLighting(root.transform);
            BuildBaseTerrain(root.transform, materials);
            BuildHexPlaza(root.transform, hexMesh, materials);
            BuildLandmarks(root.transform, spireMesh, materials);
            BuildDistricts(root.transform, materials, spireMesh);
            RaceCourseDefinition course = BuildRaceCourse(root.transform, materials);
            BuildSpawnPoints(root.transform);
            BuildPlayablePrototype(root.transform, materials, course);
            BuildOverviewCamera();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureSceneInBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Evaverse Revival Hub generated at {ScenePath} with map definition {MapDefinitionPath}.");
        }

        private static void EnsureSceneInBuildSettings()
        {
            EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;

            for (int i = 0; i < existingScenes.Length; i++)
            {
                if (!string.Equals(existingScenes[i].path, ScenePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                existingScenes[i].enabled = true;
                EditorBuildSettings.scenes = existingScenes;
                return;
            }

            EditorBuildSettingsScene[] updatedScenes = new EditorBuildSettingsScene[existingScenes.Length + 1];
            Array.Copy(existingScenes, updatedScenes, existingScenes.Length);
            updatedScenes[updatedScenes.Length - 1] = new EditorBuildSettingsScene(ScenePath, true);
            EditorBuildSettings.scenes = updatedScenes;
        }

        private static void EnsureProjectFolders()
        {
            EnsureFolder("Assets/_Project");
            EnsureFolder("Assets/_Project/Scenes");
            EnsureFolder("Assets/_Project/Settings");
            EnsureFolder("Assets/_Project/Settings/World");
            EnsureFolder("Assets/_Project/Materials");
            EnsureFolder(MaterialFolder);
            EnsureFolder("Assets/_Project/Art");
            EnsureFolder("Assets/_Project/Art/Generated");
            EnsureFolder(MeshFolder);
        }

        private static void EnsureFolder(string folderPath)
        {
            string normalized = folderPath.Replace('\\', '/');
            if (AssetDatabase.IsValidFolder(normalized))
            {
                return;
            }

            string[] parts = normalized.Split('/');
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

        private static WorldMapDefinition CreateOrUpdateMapDefinition()
        {
            WorldMapDefinition definition = AssetDatabase.LoadAssetAtPath<WorldMapDefinition>(MapDefinitionPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<WorldMapDefinition>();
                AssetDatabase.CreateAsset(definition, MapDefinitionPath);
            }

            SerializedObject serialized = new SerializedObject(definition);
            serialized.FindProperty("mapId").stringValue = "evaverse-revival-hub";
            serialized.FindProperty("displayName").stringValue = "Snowbound Nexus";
            serialized.FindProperty("mapCategory").enumValueIndex = (int)WorldMapCategory.Hub;
            serialized.FindProperty("defaultSceneName").stringValue = "Hub";
            serialized.FindProperty("worldSize").vector2Value = new Vector2(1600f, 1600f);
            serialized.FindProperty("recommendedPlayers").intValue = 48;
            serialized.FindProperty("streamingWorld").boolValue = true;

            WriteSpawns(serialized.FindProperty("spawns"));
            WriteBiomeZones(serialized.FindProperty("biomeZones"));
            WriteRegions(serialized.FindProperty("regions"));
            WritePointsOfInterest(serialized.FindProperty("pointsOfInterest"));
            WriteBlockout(serialized.FindProperty("suggestedBlockout"));

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void WriteSpawns(SerializedProperty spawns)
        {
            SpawnSpec[] specs =
            {
                new SpawnSpec("plaza-default", "Central Plaza", new Vector3(0f, 1.2f, -28f), new Vector3(0f, 0f, 0f), true, "default", "social", "safe"),
                new SpawnSpec("race-gate", "EVA Grand Prix Gate", new Vector3(0f, 1.2f, -210f), new Vector3(0f, 0f, 0f), false, "activity", "race"),
                new SpawnSpec("cosmic-stadium", "Cosmic Stadium", new Vector3(250f, 1.2f, 315f), new Vector3(0f, -140f, 0f), false, "activity", "cosmic")
            };

            spawns.arraySize = specs.Length;
            for (int i = 0; i < specs.Length; i++)
            {
                SerializedProperty element = spawns.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("spawnId").stringValue = specs[i].Id;
                element.FindPropertyRelative("label").stringValue = specs[i].Label;
                element.FindPropertyRelative("sceneName").stringValue = "Hub";
                element.FindPropertyRelative("position").vector3Value = specs[i].Position;
                element.FindPropertyRelative("eulerAngles").vector3Value = specs[i].EulerAngles;
                element.FindPropertyRelative("preferredForFirstTimePlayers").boolValue = specs[i].Preferred;

                SerializedProperty tags = element.FindPropertyRelative("tags");
                tags.arraySize = specs[i].Tags.Length;
                for (int tagIndex = 0; tagIndex < specs[i].Tags.Length; tagIndex++)
                {
                    tags.GetArrayElementAtIndex(tagIndex).stringValue = specs[i].Tags[tagIndex];
                }
            }
        }

        private static void WriteBiomeZones(SerializedProperty zones)
        {
            BiomeSpec[] specs =
            {
                new BiomeSpec("snowbound-nexus", "Snowbound Nexus", WorldBiomeType.Snow, new Bounds(Vector3.zero, new Vector3(540f, 110f, 540f)), new Color(0.2f, 0.86f, 1f), "Confirmed Original", "Restored snowy neon social hub with hoverboard and minigame access."),
                new BiomeSpec("lava-forge", "Lava Forge", WorldBiomeType.Lava, new Bounds(new Vector3(420f, 20f, 0f), new Vector3(310f, 120f, 330f)), new Color(1f, 0.36f, 0.04f), "New Evolution", "Molten arena and combat-prototype wing."),
                new BiomeSpec("jungle-canopy", "Jungle Canopy", WorldBiomeType.Jungle, new Bounds(new Vector3(-320f, 24f, 260f), new Vector3(320f, 140f, 320f)), new Color(0.16f, 0.86f, 0.28f), "New Evolution", "Treasure-hunt, pet/turtle social, and vertical traversal district."),
                new BiomeSpec("desert-dunes", "Desert Dunes", WorldBiomeType.Desert, new Bounds(new Vector3(-320f, 20f, -260f), new Vector3(320f, 110f, 320f)), new Color(1f, 0.72f, 0.24f), "New Evolution", "Open hoverboard race lines, ruins, and wind-sculpted ramps."),
                new BiomeSpec("cosmic-stadium", "Cosmic Stadium", WorldBiomeType.Cosmic, new Bounds(new Vector3(240f, 24f, 320f), new Vector3(340f, 150f, 310f)), new Color(0.94f, 0.18f, 1f), "Confirmed Original", "Cosmic Cup and low-gravity spectacle district.")
            };

            zones.arraySize = specs.Length;
            for (int i = 0; i < specs.Length; i++)
            {
                SerializedProperty element = zones.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("zoneId").stringValue = specs[i].Id;
                element.FindPropertyRelative("displayName").stringValue = specs[i].Label;
                element.FindPropertyRelative("biome").enumValueIndex = (int)specs[i].Biome;
                element.FindPropertyRelative("worldBounds").boundsValue = specs[i].Bounds;
                element.FindPropertyRelative("accentColor").colorValue = specs[i].Accent;
                element.FindPropertyRelative("canonStatus").stringValue = specs[i].CanonStatus;
                element.FindPropertyRelative("designIntent").stringValue = specs[i].DesignIntent;
            }
        }

        private static void WriteRegions(SerializedProperty regions)
        {
            RegionSpec[] specs =
            {
                new RegionSpec("central-plaza", "Central Plaza", WorldRegionPurpose.Social, new Bounds(Vector3.zero, new Vector3(360f, 48f, 360f)), false),
                new RegionSpec("race-wing", "EVA Grand Prix Wing", WorldRegionPurpose.Activity, new Bounds(new Vector3(0f, 12f, -250f), new Vector3(320f, 48f, 220f)), true),
                new RegionSpec("lava-forge", "Lava Forge", WorldRegionPurpose.Activity, new Bounds(new Vector3(420f, 16f, 0f), new Vector3(310f, 64f, 330f)), true),
                new RegionSpec("jungle-canopy", "Jungle Canopy", WorldRegionPurpose.Activity, new Bounds(new Vector3(-320f, 18f, 260f), new Vector3(320f, 96f, 320f)), true),
                new RegionSpec("desert-dunes", "Desert Dunes", WorldRegionPurpose.Activity, new Bounds(new Vector3(-320f, 16f, -260f), new Vector3(320f, 64f, 320f)), true),
                new RegionSpec("cosmic-stadium", "Cosmic Stadium", WorldRegionPurpose.Activity, new Bounds(new Vector3(240f, 20f, 320f), new Vector3(340f, 96f, 310f)), true)
            };

            regions.arraySize = specs.Length;
            for (int i = 0; i < specs.Length; i++)
            {
                SerializedProperty element = regions.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("regionId").stringValue = specs[i].Id;
                element.FindPropertyRelative("displayName").stringValue = specs[i].Label;
                element.FindPropertyRelative("sceneName").stringValue = "Hub";
                element.FindPropertyRelative("purpose").enumValueIndex = (int)specs[i].Purpose;
                element.FindPropertyRelative("worldBounds").boundsValue = specs[i].Bounds;
                element.FindPropertyRelative("loadAdditively").boolValue = specs[i].LoadAdditively;
            }
        }

        private static void WritePointsOfInterest(SerializedProperty points)
        {
            PointSpec[] specs =
            {
                new PointSpec("poi-nexus-core", "Nexus Core", WorldPointOfInterestType.Landmark, new Vector3(0f, 0f, 24f), "Central social landmark and orientation point."),
                new PointSpec("poi-grand-prix", "EVA Grand Prix", WorldPointOfInterestType.Activity, new Vector3(0f, 0f, -210f), "Restored hoverboard race entry."),
                new PointSpec("poi-cosmic-cup", "Cosmic Cup", WorldPointOfInterestType.Activity, new Vector3(240f, 0f, 320f), "Cosmic sport and low-gravity event access."),
                new PointSpec("poi-lava-forge", "Lava Forge", WorldPointOfInterestType.Portal, new Vector3(420f, 0f, 0f), "New Evolution arena district."),
                new PointSpec("poi-jungle-canopy", "Jungle Canopy", WorldPointOfInterestType.Portal, new Vector3(-320f, 0f, 260f), "New Evolution exploration district."),
                new PointSpec("poi-desert-dunes", "Desert Dunes", WorldPointOfInterestType.Portal, new Vector3(-320f, 0f, -260f), "New Evolution race district.")
            };

            points.arraySize = specs.Length;
            for (int i = 0; i < specs.Length; i++)
            {
                SerializedProperty element = points.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("pointId").stringValue = specs[i].Id;
                element.FindPropertyRelative("displayName").stringValue = specs[i].Label;
                element.FindPropertyRelative("sceneName").stringValue = "Hub";
                element.FindPropertyRelative("pointType").enumValueIndex = (int)specs[i].Type;
                element.FindPropertyRelative("position").vector3Value = specs[i].Position;
                element.FindPropertyRelative("description").stringValue = specs[i].Description;
            }
        }

        private static void WriteBlockout(SerializedProperty blockout)
        {
            blockout.arraySize = 0;
        }

        private static void ConfigureWorldComponents(GameObject worldRoot, WorldMapDefinition mapDefinition)
        {
            WorldMapAnchor anchor = worldRoot.AddComponent<WorldMapAnchor>();
            SerializedObject anchorObject = new SerializedObject(anchor);
            anchorObject.FindProperty("definition").objectReferenceValue = mapDefinition;
            anchorObject.FindProperty("fallbackSpawnId").stringValue = "plaza-default";
            anchorObject.ApplyModifiedPropertiesWithoutUndo();

            WorldMapRegistry registry = worldRoot.AddComponent<WorldMapRegistry>();
            SerializedObject registryObject = new SerializedObject(registry);
            registryObject.FindProperty("defaultMap").objectReferenceValue = mapDefinition;
            SerializedProperty maps = registryObject.FindProperty("maps");
            maps.arraySize = 1;
            maps.GetArrayElementAtIndex(0).objectReferenceValue = mapDefinition;
            registryObject.ApplyModifiedPropertiesWithoutUndo();

            TextAsset seed = AssetDatabase.LoadAssetAtPath<TextAsset>(SeedPath);
            if (seed != null)
            {
                WorldBlockoutBuilder blockoutBuilder = worldRoot.AddComponent<WorldBlockoutBuilder>();
                SerializedObject builderObject = new SerializedObject(blockoutBuilder);
                builderObject.FindProperty("seedJson").objectReferenceValue = seed;
                builderObject.FindProperty("generatedRootName").stringValue = "_GeneratedSeedPreview";
                builderObject.FindProperty("clearBeforeBuild").boolValue = true;
                builderObject.FindProperty("keepColliders").boolValue = true;
                builderObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void BuildLighting(Transform parent)
        {
            RenderSettings.ambientLight = new Color(0.36f, 0.45f, 0.56f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.74f, 0.88f, 0.96f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0022f;

            GameObject lightObject = new GameObject("Directional Light - Arctic Sunrise");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.93f, 0.98f, 1f);
            light.intensity = 1.35f;
            light.shadows = LightShadows.Soft;
        }

        private static void BuildBaseTerrain(Transform parent, MaterialLibrary materials)
        {
            Transform terrain = CreateGroup(parent, "Terrain");
            CreatePrimitive("snowfield-main", PrimitiveType.Cube, terrain, new Vector3(0f, -1.2f, 0f), Vector3.zero, new Vector3(900f, 2f, 900f), materials.Snow);
            CreatePrimitive("snowfield-rim", PrimitiveType.Cylinder, terrain, new Vector3(0f, -0.55f, 0f), Vector3.zero, new Vector3(560f, 0.45f, 560f), materials.Ice);
            CreatePrimitive("frozen-lake-glow", PrimitiveType.Cylinder, terrain, new Vector3(0f, -0.34f, 0f), Vector3.zero, new Vector3(250f, 0.12f, 250f), materials.GlassCyan);
        }

        private static void BuildHexPlaza(Transform parent, Mesh hexMesh, MaterialLibrary materials)
        {
            Transform plaza = CreateGroup(parent, "Snowbound Hex Plaza");
            int radius = 7;
            float size = 9.2f;

            for (int q = -radius; q <= radius; q++)
            {
                for (int r = -radius; r <= radius; r++)
                {
                    int s = -q - r;
                    if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) > radius)
                    {
                        continue;
                    }

                    Vector3 position = new Vector3(
                        size * Mathf.Sqrt(3f) * (q + r * 0.5f),
                        0f,
                        size * 1.5f * r);

                    int ring = Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));
                    Material tileMaterial = ring % 3 == 0 ? materials.HexDark : ring % 2 == 0 ? materials.Ice : materials.Snow;
                    GameObject tile = CreateMeshObject($"hex-tile-{q:+00;-00;00}-{r:+00;-00;00}", hexMesh, plaza, position, Vector3.zero, new Vector3(8.9f, 0.28f, 8.9f), tileMaterial);
                    tile.AddComponent<MeshCollider>();
                }
            }

            CreatePrimitive("nexus-ring-outer", PrimitiveType.Cylinder, plaza, new Vector3(0f, 0.24f, 24f), Vector3.zero, new Vector3(38f, 0.28f, 38f), materials.NeonCyan);
            CreatePrimitive("nexus-ring-inner", PrimitiveType.Cylinder, plaza, new Vector3(0f, 0.5f, 24f), Vector3.zero, new Vector3(20f, 0.26f, 20f), materials.HexDark);
        }

        private static void BuildLandmarks(Transform parent, Mesh spireMesh, MaterialLibrary materials)
        {
            Transform landmarks = CreateGroup(parent, "Landmarks");

            CreatePrimitive("nexus-core-base", PrimitiveType.Cylinder, landmarks, new Vector3(0f, 3f, 24f), Vector3.zero, new Vector3(16f, 3f, 16f), materials.MetalDark);
            CreateMeshObject("nexus-core-ice-spire", spireMesh, landmarks, new Vector3(0f, 19f, 24f), new Vector3(0f, 22f, 0f), new Vector3(18f, 34f, 18f), materials.IceSpire);
            CreatePrimitive("nexus-core-cyan-band", PrimitiveType.Cylinder, landmarks, new Vector3(0f, 8f, 24f), Vector3.zero, new Vector3(21f, 0.38f, 21f), materials.NeonCyan);
            CreatePrimitive("nexus-core-orange-band", PrimitiveType.Cylinder, landmarks, new Vector3(0f, 10.8f, 24f), Vector3.zero, new Vector3(17f, 0.34f, 17f), materials.NeonOrange);

            for (int i = 0; i < 12; i++)
            {
                float angle = i * 30f;
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                float distance = i % 2 == 0 ? 154f : 116f;
                float height = i % 3 == 0 ? 34f : 22f;
                CreateMeshObject($"ice-spire-ring-{i:00}", spireMesh, landmarks, direction * distance + Vector3.up * (height * 0.5f), new Vector3(0f, angle * 0.27f, 0f), new Vector3(8f, height, 8f), materials.IceSpire);
            }
        }

        private static void BuildDistricts(Transform parent, MaterialLibrary materials, Mesh spireMesh)
        {
            Transform districts = CreateGroup(parent, "Districts");

            CreateDistrict(districts, "EVA Grand Prix", new Vector3(0f, 0f, -210f), 0f, materials.NeonOrange, materials.HexDark, "Confirmed Original", "Hoverboard race", materials);
            CreateDistrict(districts, "Lava Forge", new Vector3(420f, 0f, 0f), -90f, materials.Lava, materials.MetalDark, "New Evolution", "Arena prototype", materials);
            CreateDistrict(districts, "Jungle Canopy", new Vector3(-320f, 0f, 260f), 135f, materials.Jungle, materials.Snow, "New Evolution", "Treasure and turtle paths", materials);
            CreateDistrict(districts, "Desert Dunes", new Vector3(-320f, 0f, -260f), 35f, materials.Desert, materials.Snow, "New Evolution", "Long-form race routes", materials);
            CreateDistrict(districts, "Cosmic Stadium", new Vector3(240f, 0f, 320f), -140f, materials.NeonMagenta, materials.HexDark, "Confirmed Original", "Cosmic Cup", materials);

            BuildLavaDressing(districts, materials);
            BuildJungleDressing(districts, materials);
            BuildDesertDressing(districts, materials);
            BuildCosmicDressing(districts, materials, spireMesh);
        }

        private static void CreateDistrict(
            Transform parent,
            string name,
            Vector3 position,
            float yaw,
            Material accent,
            Material floor,
            string canonStatus,
            string hook,
            MaterialLibrary materials)
        {
            Transform district = CreateGroup(parent, name);
            district.position = position;
            district.rotation = Quaternion.Euler(0f, yaw, 0f);

            CreatePrimitive($"{name}-landing-pad", PrimitiveType.Cylinder, district, Vector3.zero, Vector3.zero, new Vector3(46f, 0.45f, 46f), floor);
            CreatePrimitive($"{name}-portal-left", PrimitiveType.Cube, district, new Vector3(-16f, 10f, 18f), Vector3.zero, new Vector3(5f, 20f, 5f), materials.MetalDark);
            CreatePrimitive($"{name}-portal-right", PrimitiveType.Cube, district, new Vector3(16f, 10f, 18f), Vector3.zero, new Vector3(5f, 20f, 5f), materials.MetalDark);
            CreatePrimitive($"{name}-portal-top", PrimitiveType.Cube, district, new Vector3(0f, 21f, 18f), Vector3.zero, new Vector3(39f, 5f, 5f), accent);
            CreatePrimitive($"{name}-portal-shimmer", PrimitiveType.Cube, district, new Vector3(0f, 11f, 19f), Vector3.zero, new Vector3(26f, 15f, 0.55f), accent);
            CreatePrimitive($"{name}-activity-pad", PrimitiveType.Cylinder, district, new Vector3(0f, 0.5f, -30f), Vector3.zero, new Vector3(22f, 0.3f, 22f), accent);
            CreatePrimitive($"{name}-left-rail", PrimitiveType.Cube, district, new Vector3(-26f, 2f, -14f), new Vector3(0f, 0f, 0f), new Vector3(3f, 4f, 62f), accent);
            CreatePrimitive($"{name}-right-rail", PrimitiveType.Cube, district, new Vector3(26f, 2f, -14f), new Vector3(0f, 0f, 0f), new Vector3(3f, 4f, 62f), accent);

            CreateWorldLabel($"{name}-label", district, new Vector3(0f, 28f, 16f), $"{name}\n{canonStatus}\n{hook}", accent.color);
        }

        private static void BuildLavaDressing(Transform parent, MaterialLibrary materials)
        {
            Transform lava = parent.Find("Lava Forge");
            if (lava == null)
            {
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                CreatePrimitive($"lava-channel-{i:00}", PrimitiveType.Cube, lava, new Vector3(-42f + i * 21f, -0.1f, -54f), Vector3.zero, new Vector3(12f, 0.35f, 44f), materials.Lava);
            }
        }

        private static void BuildJungleDressing(Transform parent, MaterialLibrary materials)
        {
            Transform jungle = parent.Find("Jungle Canopy");
            if (jungle == null)
            {
                return;
            }

            for (int i = 0; i < 9; i++)
            {
                float angle = i * 40f;
                Vector3 local = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, 54f + (i % 3) * 8f);
                CreatePrimitive($"jungle-trunk-{i:00}", PrimitiveType.Cylinder, jungle, local + Vector3.up * 9f, Vector3.zero, new Vector3(4f, 9f, 4f), materials.MetalDark);
                CreatePrimitive($"jungle-canopy-{i:00}", PrimitiveType.Sphere, jungle, local + Vector3.up * 21f, Vector3.zero, new Vector3(17f, 9f, 17f), materials.Jungle);
            }
        }

        private static void BuildDesertDressing(Transform parent, MaterialLibrary materials)
        {
            Transform desert = parent.Find("Desert Dunes");
            if (desert == null)
            {
                return;
            }

            for (int i = 0; i < 6; i++)
            {
                CreatePrimitive($"dune-ridge-{i:00}", PrimitiveType.Cube, desert, new Vector3(-60f + i * 24f, 1.1f, -58f - i * 3f), new Vector3(0f, 14f, 6f), new Vector3(38f, 2.2f, 8f), materials.Desert);
            }
        }

        private static void BuildCosmicDressing(Transform parent, MaterialLibrary materials, Mesh spireMesh)
        {
            Transform cosmic = parent.Find("Cosmic Stadium");
            if (cosmic == null)
            {
                return;
            }

            CreatePrimitive("cosmic-cup-field", PrimitiveType.Cylinder, cosmic, new Vector3(0f, 0.7f, -62f), Vector3.zero, new Vector3(62f, 0.45f, 62f), materials.GlassCyan);

            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f;
                Vector3 local = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, 72f);
                CreateMeshObject($"cosmic-orbital-pylon-{i:00}", spireMesh, cosmic, local + Vector3.up * 18f, Vector3.zero, new Vector3(6f, 30f, 6f), materials.NeonMagenta);
            }
        }

        private static RaceCourseDefinition BuildRaceCourse(Transform parent, MaterialLibrary materials)
        {
            Transform raceRoot = CreateGroup(parent, "Playable Race Prototype");
            RaceCourseDefinition course = raceRoot.gameObject.AddComponent<RaceCourseDefinition>();
            Vector3[] checkpoints =
            {
                new Vector3(0f, 3f, -185f),
                new Vector3(90f, 3f, -285f),
                new Vector3(0f, 3f, -390f),
                new Vector3(-90f, 3f, -285f)
            };

            List<RaceCheckpoint> checkpointComponents = new List<RaceCheckpoint>();
            CreateStartGate(raceRoot, new Vector3(0f, 3.5f, -145f), materials.Jungle);

            for (int i = 0; i < checkpoints.Length; i++)
            {
                GameObject checkpoint = CreatePrimitive($"race-checkpoint-{i:00}", PrimitiveType.Cube, raceRoot, checkpoints[i], Vector3.zero, new Vector3(30f, 7f, 3f), materials.NeonCyan);
                BoxCollider collider = checkpoint.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                RaceCheckpoint raceCheckpoint = checkpoint.AddComponent<RaceCheckpoint>();
                SerializedObject checkpointObject = new SerializedObject(raceCheckpoint);
                checkpointObject.FindProperty("index").intValue = i;
                checkpointObject.ApplyModifiedPropertiesWithoutUndo();

                checkpointComponents.Add(raceCheckpoint);
                CreateCheckpointGate(raceRoot, $"race-checkpoint-gate-{i:00}", checkpoints[i], i * 35f, materials.NeonOrange);
            }

            SerializedObject courseObject = new SerializedObject(course);
            courseObject.FindProperty("courseId").stringValue = "eva-grand-prix-snowbound-loop";
            courseObject.FindProperty("lapCount").intValue = 3;
            SerializedProperty checkpointList = courseObject.FindProperty("checkpoints");
            checkpointList.arraySize = checkpointComponents.Count;
            for (int i = 0; i < checkpointComponents.Count; i++)
            {
                checkpointList.GetArrayElementAtIndex(i).objectReferenceValue = checkpointComponents[i];
            }

            courseObject.ApplyModifiedPropertiesWithoutUndo();
            return course;
        }

        private static void CreateStartGate(Transform parent, Vector3 position, Material material)
        {
            Transform gate = CreateGroup(parent, "race-start-gate");
            gate.localPosition = position;

            GameObject trigger = CreatePrimitive("race-start-trigger", PrimitiveType.Cube, gate, Vector3.zero, Vector3.zero, new Vector3(34f, 8f, 5f), material);
            BoxCollider triggerCollider = trigger.GetComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            trigger.AddComponent<RaceStartGate>();

            CreatePrimitive("race-start-left", PrimitiveType.Cube, gate, new Vector3(-18f, 6f, 0f), Vector3.zero, new Vector3(2.5f, 12f, 2.5f), material);
            CreatePrimitive("race-start-right", PrimitiveType.Cube, gate, new Vector3(18f, 6f, 0f), Vector3.zero, new Vector3(2.5f, 12f, 2.5f), material);
            CreatePrimitive("race-start-top", PrimitiveType.Cube, gate, new Vector3(0f, 13.5f, 0f), Vector3.zero, new Vector3(39f, 2.5f, 2.5f), material);
            CreateWorldLabel("race-start-label", gate, new Vector3(0f, 18f, 0f), "START\nCountdown Gate", material.color);
        }

        private static void CreateCheckpointGate(Transform parent, string name, Vector3 position, float yaw, Material material)
        {
            Transform gate = CreateGroup(parent, name);
            gate.localPosition = position;
            gate.localRotation = Quaternion.Euler(0f, yaw, 0f);

            CreatePrimitive($"{name}-left", PrimitiveType.Cube, gate, new Vector3(-14f, 6f, 0f), Vector3.zero, new Vector3(2.2f, 12f, 2.2f), material);
            CreatePrimitive($"{name}-right", PrimitiveType.Cube, gate, new Vector3(14f, 6f, 0f), Vector3.zero, new Vector3(2.2f, 12f, 2.2f), material);
            CreatePrimitive($"{name}-top", PrimitiveType.Cube, gate, new Vector3(0f, 12.8f, 0f), Vector3.zero, new Vector3(31f, 2.2f, 2.2f), material);
        }

        private static void BuildSpawnPoints(Transform parent)
        {
            Transform spawns = CreateGroup(parent, "Spawn Points");
            CreateSpawn(spawns, "plaza-default", "Central Plaza", new Vector3(0f, 1.2f, -28f), Vector3.zero, true, new[] { "default", "social", "safe" });
            CreateSpawn(spawns, "race-gate", "EVA Grand Prix Gate", new Vector3(0f, 1.2f, -210f), Vector3.zero, false, new[] { "activity", "race" });
            CreateSpawn(spawns, "cosmic-stadium", "Cosmic Stadium", new Vector3(250f, 1.2f, 315f), new Vector3(0f, -140f, 0f), false, new[] { "activity", "cosmic" });
        }

        private static void CreateSpawn(Transform parent, string spawnId, string label, Vector3 position, Vector3 eulerAngles, bool preferred, string[] tags)
        {
            GameObject spawn = new GameObject($"spawn-{spawnId}");
            spawn.transform.SetParent(parent, false);
            spawn.transform.position = position;
            spawn.transform.rotation = Quaternion.Euler(eulerAngles);

            WorldSpawnPoint point = spawn.AddComponent<WorldSpawnPoint>();
            SerializedObject serialized = new SerializedObject(point);
            serialized.FindProperty("spawnId").stringValue = spawnId;
            serialized.FindProperty("label").stringValue = label;
            serialized.FindProperty("sceneNameOverride").stringValue = "Hub";
            serialized.FindProperty("preferredForFirstTimePlayers").boolValue = preferred;

            SerializedProperty tagList = serialized.FindProperty("tags");
            tagList.arraySize = tags.Length;
            for (int i = 0; i < tags.Length; i++)
            {
                tagList.GetArrayElementAtIndex(i).stringValue = tags[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildPlayablePrototype(Transform parent, MaterialLibrary materials, RaceCourseDefinition course)
        {
            Transform prototype = CreateGroup(parent, "Playable Prototype");

            GameObject player = new GameObject("Local Player Prototype");
            player.transform.SetParent(prototype, false);
            player.transform.localPosition = new Vector3(0f, 1.25f, -28f);

            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.36f;
            controller.center = new Vector3(0f, 1f, 0f);

            GameObject racingProbe = new GameObject("Racing Trigger Probe");
            racingProbe.transform.SetParent(player.transform, false);
            racingProbe.transform.localPosition = new Vector3(0f, 1f, 0f);
            Rigidbody probeBody = racingProbe.AddComponent<Rigidbody>();
            probeBody.isKinematic = true;
            probeBody.useGravity = false;
            CapsuleCollider probeCollider = racingProbe.AddComponent<CapsuleCollider>();
            probeCollider.isTrigger = true;
            probeCollider.radius = 0.45f;
            probeCollider.height = 1.9f;
            probeCollider.direction = 1;
            probeCollider.center = Vector3.zero;

            AvatarMotor avatarMotor = player.AddComponent<AvatarMotor>();
            RaceLapTracker playerTracker = player.AddComponent<RaceLapTracker>();

            GameObject avatarVisual = CreatePrimitive("avatar-capsule-visual", PrimitiveType.Capsule, player.transform, new Vector3(0f, 1f, 0f), Vector3.zero, new Vector3(0.72f, 1f, 0.72f), materials.NeonCyan);
            MarkDynamic(avatarVisual);
            RemoveCollider(avatarVisual);

            GameObject visor = CreatePrimitive("avatar-visor", PrimitiveType.Cube, player.transform, new Vector3(0f, 1.45f, 0.33f), Vector3.zero, new Vector3(0.5f, 0.12f, 0.08f), materials.NeonOrange);
            MarkDynamic(visor);
            RemoveCollider(visor);

            GameObject pivot = new GameObject("Camera Pivot");
            pivot.transform.SetParent(player.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 1.68f, 0f);

            GameObject playerCamera = new GameObject("Main Camera");
            playerCamera.tag = "MainCamera";
            playerCamera.transform.SetParent(player.transform, false);
            Camera camera = playerCamera.AddComponent<Camera>();
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.2f;
            camera.farClipPlane = 1800f;
            playerCamera.AddComponent<AudioListener>();

            SerializedObject avatarObject = new SerializedObject(avatarMotor);
            avatarObject.FindProperty("cameraPivot").objectReferenceValue = pivot.transform;
            avatarObject.ApplyModifiedPropertiesWithoutUndo();
            AssignRaceCourse(playerTracker, course);

            GameObject board = new GameObject("Prototype Hoverboard");
            board.transform.SetParent(prototype, false);
            board.transform.localPosition = new Vector3(5f, 1.45f, -28f);
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

            HoverboardMount boardMount = board.AddComponent<HoverboardMount>();

            GameObject boardDeck = CreatePrimitive("hoverboard-deck-visual", PrimitiveType.Cube, board.transform, new Vector3(0f, 0.2f, 0f), Vector3.zero, new Vector3(2.8f, 0.22f, 6.2f), materials.MetalDark);
            MarkDynamic(boardDeck);
            RemoveCollider(boardDeck);
            GameObject boardTrim = CreatePrimitive("hoverboard-neon-trim", PrimitiveType.Cube, board.transform, new Vector3(0f, 0.36f, 0f), Vector3.zero, new Vector3(3.05f, 0.08f, 6.5f), materials.NeonOrange);
            MarkDynamic(boardTrim);
            RemoveCollider(boardTrim);

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

            SerializedObject mountObject = new SerializedObject(boardMount);
            mountObject.FindProperty("riderSocket").objectReferenceValue = riderSocket.transform;
            mountObject.ApplyModifiedPropertiesWithoutUndo();
            HoverboardMountController mountController = player.AddComponent<HoverboardMountController>();
            SerializedObject mountControllerObject = new SerializedObject(mountController);
            mountControllerObject.FindProperty("avatarController").objectReferenceValue = controller;
            mountControllerObject.FindProperty("hoverboardMotor").objectReferenceValue = boardMotor;
            mountControllerObject.FindProperty("hoverboardMount").objectReferenceValue = boardMount;
            mountControllerObject.FindProperty("rider").objectReferenceValue = player.transform;
            mountControllerObject.FindProperty("dismountPoint").objectReferenceValue = dismountPoint.transform;
            mountControllerObject.FindProperty("mountDistance").floatValue = 7f;
            mountControllerObject.ApplyModifiedPropertiesWithoutUndo();

            CinemachinePlayerRig playerRig = player.AddComponent<CinemachinePlayerRig>();
            SerializedObject rigObject = new SerializedObject(playerRig);
            rigObject.FindProperty("playerCamera").objectReferenceValue = camera;
            rigObject.FindProperty("cameraPivot").objectReferenceValue = pivot.transform;
            rigObject.FindProperty("mountController").objectReferenceValue = mountController;
            rigObject.ApplyModifiedPropertiesWithoutUndo();

            GameObject hudObject = new GameObject("Race Prototype HUD");
            hudObject.transform.SetParent(prototype, false);
            RacePrototypeHud hud = hudObject.AddComponent<RacePrototypeHud>();
            SerializedObject hudObjectSerialized = new SerializedObject(hud);
            hudObjectSerialized.FindProperty("tracker").objectReferenceValue = playerTracker;
            hudObjectSerialized.FindProperty("hoverboard").objectReferenceValue = boardMotor;
            hudObjectSerialized.FindProperty("showControls").boolValue = true;
            hudObjectSerialized.ApplyModifiedPropertiesWithoutUndo();

            CreateWorldLabel("prototype-controls-label", prototype, new Vector3(0f, 8f, -48f), "Prototype Controls\nWASD / left stick — move & ride\nShift / L3 — sprint & drift\nSpace / A — jump & boost\nE / X — mount near board\nEsc — free cursor", Color.white);
        }

        private static void AssignRaceCourse(RaceLapTracker tracker, RaceCourseDefinition course)
        {
            if (tracker == null || course == null)
            {
                return;
            }

            SerializedObject trackerObject = new SerializedObject(tracker);
            trackerObject.FindProperty("course").objectReferenceValue = course;
            trackerObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildOverviewCamera()
        {
            GameObject cameraObject = new GameObject("Overview Camera");
            cameraObject.transform.position = new Vector3(0f, 155f, -280f);
            cameraObject.transform.rotation = Quaternion.Euler(56f, 0f, 0f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 2200f;
            camera.enabled = false;
        }

        private static Transform CreateGroup(Transform parent, string name)
        {
            GameObject group = new GameObject(name);
            group.transform.SetParent(parent, false);
            return group.transform;
        }

        private static GameObject CreatePrimitive(
            string name,
            PrimitiveType primitive,
            Transform parent,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Material material)
        {
            GameObject instance = GameObject.CreatePrimitive(primitive);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = Quaternion.Euler(rotationEuler);
            instance.transform.localScale = scale;
            ApplyMaterial(instance, material);
            GameObjectUtility.SetStaticEditorFlags(instance, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            return instance;
        }

        private static GameObject CreateMeshObject(
            string name,
            Mesh mesh,
            Transform parent,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Material material)
        {
            GameObject instance = new GameObject(name);
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = Quaternion.Euler(rotationEuler);
            instance.transform.localScale = scale;

            MeshFilter filter = instance.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = instance.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;

            GameObjectUtility.SetStaticEditorFlags(instance, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
            return instance;
        }

        private static void ApplyMaterial(GameObject instance, Material material)
        {
            if (material == null || !instance.TryGetComponent(out Renderer renderer))
            {
                return;
            }

            renderer.sharedMaterial = material;
        }

        private static void RemoveCollider(GameObject instance)
        {
            if (instance.TryGetComponent(out Collider collider))
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }
        }

        private static void MarkDynamic(GameObject instance)
        {
            GameObjectUtility.SetStaticEditorFlags(instance, 0);
        }

        private static void CreateWorldLabel(string name, Transform parent, Vector3 localPosition, string text, Color color)
        {
            GameObject label = new GameObject(name);
            label.transform.SetParent(parent, false);
            label.transform.localPosition = localPosition;
            label.transform.localRotation = Quaternion.Euler(68f, 0f, 0f);

            TextMesh mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.characterSize = 3.6f;
            mesh.fontSize = 42;
            mesh.color = color;
        }

        private static Mesh CreateOrUpdateMesh(string assetPath, Func<Mesh> factory)
        {
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (mesh != null)
            {
                return mesh;
            }

            mesh = factory();
            AssetDatabase.CreateAsset(mesh, assetPath);
            return mesh;
        }

        private static Mesh CreateHexPrismMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Evaverse Hex Prism";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            vertices.Add(new Vector3(0f, 0.5f, 0f));
            vertices.Add(new Vector3(0f, -0.5f, 0f));

            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60f * i + 30f);
                vertices.Add(new Vector3(Mathf.Cos(angle), 0.5f, Mathf.Sin(angle)));
                vertices.Add(new Vector3(Mathf.Cos(angle), -0.5f, Mathf.Sin(angle)));
            }

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                int top = 2 + i * 2;
                int bottom = top + 1;
                int nextTop = 2 + next * 2;
                int nextBottom = nextTop + 1;

                triangles.Add(0);
                triangles.Add(top);
                triangles.Add(nextTop);

                triangles.Add(1);
                triangles.Add(nextBottom);
                triangles.Add(bottom);

                triangles.Add(top);
                triangles.Add(bottom);
                triangles.Add(nextBottom);

                triangles.Add(top);
                triangles.Add(nextBottom);
                triangles.Add(nextTop);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateSpireMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Evaverse Ice Spire";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            vertices.Add(new Vector3(0f, 0.5f, 0f));
            vertices.Add(new Vector3(0f, -0.5f, 0f));

            int segments = 7;
            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * (360f / segments * i);
                vertices.Add(new Vector3(Mathf.Cos(angle) * 0.45f, -0.5f, Mathf.Sin(angle) * 0.45f));
            }

            for (int i = 0; i < segments; i++)
            {
                int current = 2 + i;
                int next = 2 + ((i + 1) % segments);

                triangles.Add(0);
                triangles.Add(current);
                triangles.Add(next);

                triangles.Add(1);
                triangles.Add(next);
                triangles.Add(current);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private sealed class MaterialLibrary
        {
            public Material Snow { get; private set; }
            public Material Ice { get; private set; }
            public Material IceSpire { get; private set; }
            public Material HexDark { get; private set; }
            public Material MetalDark { get; private set; }
            public Material NeonCyan { get; private set; }
            public Material NeonOrange { get; private set; }
            public Material NeonMagenta { get; private set; }
            public Material Lava { get; private set; }
            public Material Jungle { get; private set; }
            public Material Desert { get; private set; }
            public Material GlassCyan { get; private set; }

            public static MaterialLibrary Create(string folder)
            {
                return new MaterialLibrary
                {
                    Snow = CreateMaterial($"{folder}/evaverse_snow.mat", new Color(0.91f, 0.96f, 1f), null, 0f, 0.78f),
                    Ice = CreateMaterial($"{folder}/evaverse_blue_ice.mat", new Color(0.64f, 0.9f, 1f), new Color(0.08f, 0.45f, 0.75f), 0f, 0.92f),
                    IceSpire = CreateMaterial($"{folder}/evaverse_ice_spire.mat", new Color(0.73f, 0.96f, 1f), new Color(0.18f, 0.78f, 1f), 0f, 0.95f),
                    HexDark = CreateMaterial($"{folder}/evaverse_hex_dark.mat", new Color(0.035f, 0.06f, 0.085f), null, 0.2f, 0.8f),
                    MetalDark = CreateMaterial($"{folder}/evaverse_dark_metal.mat", new Color(0.055f, 0.064f, 0.075f), null, 0.45f, 0.72f),
                    NeonCyan = CreateMaterial($"{folder}/evaverse_neon_cyan.mat", new Color(0.04f, 0.93f, 1f), new Color(0.02f, 1.25f, 1.65f), 0f, 0.9f),
                    NeonOrange = CreateMaterial($"{folder}/evaverse_neon_orange.mat", new Color(1f, 0.45f, 0.08f), new Color(1.8f, 0.55f, 0.06f), 0f, 0.86f),
                    NeonMagenta = CreateMaterial($"{folder}/evaverse_neon_magenta.mat", new Color(0.95f, 0.1f, 1f), new Color(1.4f, 0.08f, 1.9f), 0f, 0.9f),
                    Lava = CreateMaterial($"{folder}/evaverse_lava.mat", new Color(1f, 0.23f, 0.035f), new Color(2f, 0.36f, 0.02f), 0f, 0.75f),
                    Jungle = CreateMaterial($"{folder}/evaverse_jungle.mat", new Color(0.12f, 0.68f, 0.28f), new Color(0.03f, 0.35f, 0.1f), 0f, 0.62f),
                    Desert = CreateMaterial($"{folder}/evaverse_desert.mat", new Color(1f, 0.73f, 0.3f), null, 0f, 0.58f),
                    GlassCyan = CreateMaterial($"{folder}/evaverse_glass_cyan.mat", new Color(0.28f, 0.94f, 1f, 0.7f), new Color(0.02f, 0.72f, 1.2f), 0f, 0.96f)
                };
            }

            private static Material CreateMaterial(string path, Color baseColor, Color? emission, float metallic, float smoothness)
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, path);
                }

                SetColor(material, "_BaseColor", "_Color", baseColor);
                SetFloat(material, "_Metallic", metallic);
                SetFloat(material, "_Smoothness", smoothness);

                if (emission.HasValue)
                {
                    material.EnableKeyword("_EMISSION");
                    SetColor(material, "_EmissionColor", "_EmissionColor", emission.Value);
                }

                EditorUtility.SetDirty(material);
                return material;
            }

            private static void SetColor(Material material, string preferredProperty, string fallbackProperty, Color color)
            {
                if (material.HasProperty(preferredProperty))
                {
                    material.SetColor(preferredProperty, color);
                }
                else if (material.HasProperty(fallbackProperty))
                {
                    material.SetColor(fallbackProperty, color);
                }
            }

            private static void SetFloat(Material material, string property, float value)
            {
                if (material.HasProperty(property))
                {
                    material.SetFloat(property, value);
                }
            }
        }

        private readonly struct SpawnSpec
        {
            public SpawnSpec(string id, string label, Vector3 position, Vector3 eulerAngles, bool preferred, params string[] tags)
            {
                Id = id;
                Label = label;
                Position = position;
                EulerAngles = eulerAngles;
                Preferred = preferred;
                Tags = tags;
            }

            public string Id { get; }
            public string Label { get; }
            public Vector3 Position { get; }
            public Vector3 EulerAngles { get; }
            public bool Preferred { get; }
            public string[] Tags { get; }
        }

        private readonly struct BiomeSpec
        {
            public BiomeSpec(string id, string label, WorldBiomeType biome, Bounds bounds, Color accent, string canonStatus, string designIntent)
            {
                Id = id;
                Label = label;
                Biome = biome;
                Bounds = bounds;
                Accent = accent;
                CanonStatus = canonStatus;
                DesignIntent = designIntent;
            }

            public string Id { get; }
            public string Label { get; }
            public WorldBiomeType Biome { get; }
            public Bounds Bounds { get; }
            public Color Accent { get; }
            public string CanonStatus { get; }
            public string DesignIntent { get; }
        }

        private readonly struct RegionSpec
        {
            public RegionSpec(string id, string label, WorldRegionPurpose purpose, Bounds bounds, bool loadAdditively)
            {
                Id = id;
                Label = label;
                Purpose = purpose;
                Bounds = bounds;
                LoadAdditively = loadAdditively;
            }

            public string Id { get; }
            public string Label { get; }
            public WorldRegionPurpose Purpose { get; }
            public Bounds Bounds { get; }
            public bool LoadAdditively { get; }
        }

        private readonly struct PointSpec
        {
            public PointSpec(string id, string label, WorldPointOfInterestType type, Vector3 position, string description)
            {
                Id = id;
                Label = label;
                Type = type;
                Position = position;
                Description = description;
            }

            public string Id { get; }
            public string Label { get; }
            public WorldPointOfInterestType Type { get; }
            public Vector3 Position { get; }
            public string Description { get; }
        }
    }
}
