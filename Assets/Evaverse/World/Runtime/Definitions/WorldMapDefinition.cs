using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evaverse.World.Runtime.Definitions
{
    public enum WorldMapCategory
    {
        Hub,
        Race,
        Social,
        Arena,
        Event,
        Prototype
    }

    public enum WorldRegionPurpose
    {
        Social,
        Transit,
        Activity,
        Service,
        Backstage,
        Secret
    }

    public enum WorldBiomeType
    {
        Snow,
        Lava,
        Jungle,
        Desert,
        Cosmic,
        Urban,
        Void
    }

    public enum WorldPointOfInterestType
    {
        Spawn,
        Portal,
        Activity,
        Shop,
        Landmark,
        Event,
        Service
    }

    public enum WorldBlockoutPrimitiveType
    {
        Cube,
        Plane,
        Cylinder,
        Sphere,
        Capsule
    }

    [CreateAssetMenu(menuName = "Evaverse/World/Map Definition", fileName = "WorldMapDefinition")]
    public sealed class WorldMapDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string mapId = "hub-01";
        [SerializeField] private string displayName = "Evaverse Hub";
        [SerializeField] private WorldMapCategory mapCategory = WorldMapCategory.Hub;
        [SerializeField] private string defaultSceneName = "Hub";

        [Header("Scale")]
        [SerializeField] private Vector2 worldSize = new(1024f, 1024f);
        [SerializeField] private int recommendedPlayers = 32;
        [SerializeField] private bool streamingWorld = true;

        [Header("Authoring")]
        [SerializeField] private List<WorldSpawnDefinition> spawns = new()
        {
            new WorldSpawnDefinition()
        };

        [SerializeField] private List<WorldBiomeZoneDefinition> biomeZones = new();
        [SerializeField] private List<WorldRegionDefinition> regions = new();
        [SerializeField] private List<WorldPointOfInterestDefinition> pointsOfInterest = new();
        [SerializeField] private List<WorldBlockoutElementDefinition> suggestedBlockout = new();

        public string MapId => mapId;
        public string DisplayName => displayName;
        public WorldMapCategory MapCategory => mapCategory;
        public string DefaultSceneName => defaultSceneName;
        public Vector2 WorldSize => worldSize;
        public int RecommendedPlayers => Mathf.Max(1, recommendedPlayers);
        public bool StreamingWorld => streamingWorld;
        public IReadOnlyList<WorldSpawnDefinition> Spawns => spawns;
        public IReadOnlyList<WorldBiomeZoneDefinition> BiomeZones => biomeZones;
        public IReadOnlyList<WorldRegionDefinition> Regions => regions;
        public IReadOnlyList<WorldPointOfInterestDefinition> PointsOfInterest => pointsOfInterest;
        public IReadOnlyList<WorldBlockoutElementDefinition> SuggestedBlockout => suggestedBlockout;

        public bool TryGetSpawn(string spawnId, out WorldSpawnDefinition spawn)
        {
            for (int i = 0; i < spawns.Count; i++)
            {
                WorldSpawnDefinition candidate = spawns[i];
                if (!string.Equals(candidate.SpawnId, spawnId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                spawn = candidate;
                return true;
            }

            spawn = null;
            return false;
        }

        public WorldSpawnDefinition GetBestSpawn(string preferredSpawnId = null)
        {
            if (!string.IsNullOrWhiteSpace(preferredSpawnId) && TryGetSpawn(preferredSpawnId, out WorldSpawnDefinition spawn))
            {
                return spawn;
            }

            for (int i = 0; i < spawns.Count; i++)
            {
                if (spawns[i].PreferredForFirstTimePlayers)
                {
                    return spawns[i];
                }
            }

            return spawns.Count > 0 ? spawns[0] : null;
        }

        public Pose ResolveSpawnPose(string preferredSpawnId = null)
        {
            WorldSpawnDefinition spawn = GetBestSpawn(preferredSpawnId);
            return spawn != null ? spawn.CreatePose() : default;
        }

        public bool TryGetRegion(Vector3 worldPosition, out WorldRegionDefinition region)
        {
            for (int i = 0; i < regions.Count; i++)
            {
                if (!regions[i].Contains(worldPosition))
                {
                    continue;
                }

                region = regions[i];
                return true;
            }

            region = null;
            return false;
        }

        public bool TryGetBiomeZone(Vector3 worldPosition, out WorldBiomeZoneDefinition zone)
        {
            for (int i = 0; i < biomeZones.Count; i++)
            {
                if (!biomeZones[i].Contains(worldPosition))
                {
                    continue;
                }

                zone = biomeZones[i];
                return true;
            }

            zone = null;
            return false;
        }

        public string ResolveSceneName(Vector3 worldPosition)
        {
            return TryGetRegion(worldPosition, out WorldRegionDefinition region) && !string.IsNullOrWhiteSpace(region.SceneName)
                ? region.SceneName
                : defaultSceneName;
        }

        private void OnValidate()
        {
            worldSize.x = Mathf.Max(32f, worldSize.x);
            worldSize.y = Mathf.Max(32f, worldSize.y);
            recommendedPlayers = Mathf.Max(1, recommendedPlayers);
        }
    }

    [Serializable]
    public sealed class WorldBiomeZoneDefinition
    {
        [SerializeField] private string zoneId = "snowbound-nexus";
        [SerializeField] private string displayName = "Snowbound Nexus";
        [SerializeField] private WorldBiomeType biome = WorldBiomeType.Snow;
        [SerializeField] private Bounds worldBounds = new(Vector3.zero, new Vector3(384f, 80f, 384f));
        [SerializeField] private Color accentColor = new(0.2f, 0.85f, 1f, 1f);
        [SerializeField] private string canonStatus = "Confirmed Original";
        [SerializeField] private string designIntent = "Snowy sci-fi social hub inspired by original Evaverse references.";

        public string ZoneId => zoneId;
        public string DisplayName => displayName;
        public WorldBiomeType Biome => biome;
        public Bounds WorldBounds => worldBounds;
        public Color AccentColor => accentColor;
        public string CanonStatus => canonStatus;
        public string DesignIntent => designIntent;

        public bool Contains(Vector3 worldPosition)
        {
            return worldBounds.Contains(worldPosition);
        }
    }

    [Serializable]
    public sealed class WorldSpawnDefinition
    {
        [SerializeField] private string spawnId = "default";
        [SerializeField] private string label = "Default Spawn";
        [SerializeField] private string sceneName = "Hub";
        [SerializeField] private Vector3 position = new(0f, 1f, 0f);
        [SerializeField] private Vector3 eulerAngles;
        [SerializeField] private bool preferredForFirstTimePlayers = true;
        [SerializeField] private List<string> tags = new();

        public string SpawnId => spawnId;
        public string Label => label;
        public string SceneName => sceneName;
        public Vector3 Position => position;
        public Vector3 EulerAngles => eulerAngles;
        public bool PreferredForFirstTimePlayers => preferredForFirstTimePlayers;
        public IReadOnlyList<string> Tags => tags;

        public Pose CreatePose()
        {
            return new Pose(position, Quaternion.Euler(eulerAngles));
        }

        public bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            for (int i = 0; i < tags.Count; i++)
            {
                if (string.Equals(tags[i], tag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class WorldRegionDefinition
    {
        [SerializeField] private string regionId = "region-01";
        [SerializeField] private string displayName = "Central Plaza";
        [SerializeField] private string sceneName = "Hub";
        [SerializeField] private WorldRegionPurpose purpose = WorldRegionPurpose.Social;
        [SerializeField] private Bounds worldBounds = new(Vector3.zero, new Vector3(256f, 32f, 256f));
        [SerializeField] private bool loadAdditively = true;

        public string RegionId => regionId;
        public string DisplayName => displayName;
        public string SceneName => sceneName;
        public WorldRegionPurpose Purpose => purpose;
        public Bounds WorldBounds => worldBounds;
        public bool LoadAdditively => loadAdditively;

        public bool Contains(Vector3 worldPosition)
        {
            return worldBounds.Contains(worldPosition);
        }
    }

    [Serializable]
    public sealed class WorldPointOfInterestDefinition
    {
        [SerializeField] private string pointId = "poi-01";
        [SerializeField] private string displayName = "Portal";
        [SerializeField] private string sceneName = "Hub";
        [SerializeField] private WorldPointOfInterestType pointType = WorldPointOfInterestType.Landmark;
        [SerializeField] private Vector3 position;
        [SerializeField] private string description;

        public string PointId => pointId;
        public string DisplayName => displayName;
        public string SceneName => sceneName;
        public WorldPointOfInterestType PointType => pointType;
        public Vector3 Position => position;
        public string Description => description;
    }

    [Serializable]
    public sealed class WorldBlockoutElementDefinition
    {
        [SerializeField] private string elementId = "block-01";
        [SerializeField] private WorldBlockoutPrimitiveType primitive = WorldBlockoutPrimitiveType.Cube;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotationEuler;
        [SerializeField] private Vector3 scale = Vector3.one;
        [SerializeField] private string materialHint = "default";

        public string ElementId => elementId;
        public WorldBlockoutPrimitiveType Primitive => primitive;
        public Vector3 Position => position;
        public Vector3 RotationEuler => rotationEuler;
        public Vector3 Scale => scale;
        public string MaterialHint => materialHint;
    }
}
