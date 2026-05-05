using System;
using UnityEngine;

namespace Evaverse.World.Runtime.Seeds
{
    [Serializable]
    public sealed class WorldMapSeedPayload
    {
        public WorldMapSeedMetadata metadata = new();
        public WorldMapSeedSpawn[] spawns = Array.Empty<WorldMapSeedSpawn>();
        public WorldMapSeedBiomeZone[] biomeZones = Array.Empty<WorldMapSeedBiomeZone>();
        public WorldMapSeedRegion[] regions = Array.Empty<WorldMapSeedRegion>();
        public WorldMapSeedPointOfInterest[] pointsOfInterest = Array.Empty<WorldMapSeedPointOfInterest>();
        public WorldMapSeedBlockout[] blockout = Array.Empty<WorldMapSeedBlockout>();
    }

    [Serializable]
    public sealed class WorldMapSeedMetadata
    {
        public string mapId = "hub-01";
        public string displayName = "Evaverse Hub";
        public string defaultSceneName = "Hub";
        public float width = 1024f;
        public float length = 1024f;
        public int recommendedPlayers = 32;
        public bool streamingWorld = true;
        public string mapCategory = "Hub";
    }

    [Serializable]
    public sealed class WorldMapSeedSpawn
    {
        public string spawnId = "default";
        public string label = "Default Spawn";
        public string sceneName = "Hub";
        public Vector3 position = new(0f, 1f, 0f);
        public Vector3 eulerAngles;
        public bool preferredForFirstTimePlayers = true;
        public string[] tags = Array.Empty<string>();
    }

    [Serializable]
    public sealed class WorldMapSeedBiomeZone
    {
        public string zoneId = "snowbound-nexus";
        public string displayName = "Snowbound Nexus";
        public string biome = "Snow";
        public Vector3 center;
        public Vector3 size = new(384f, 80f, 384f);
        public Color accentColor = new(0.2f, 0.85f, 1f, 1f);
        public string canonStatus = "Confirmed Original";
        public string designIntent = "Snowy sci-fi social hub inspired by original Evaverse references.";
    }

    [Serializable]
    public sealed class WorldMapSeedRegion
    {
        public string regionId = "region-01";
        public string displayName = "Central Plaza";
        public string sceneName = "Hub";
        public string purpose = "Social";
        public Vector3 center;
        public Vector3 size = new(256f, 32f, 256f);
        public bool loadAdditively = true;
    }

    [Serializable]
    public sealed class WorldMapSeedPointOfInterest
    {
        public string pointId = "poi-01";
        public string displayName = "Portal";
        public string sceneName = "Hub";
        public string pointType = "Landmark";
        public Vector3 position;
        public string description = string.Empty;
    }

    [Serializable]
    public sealed class WorldMapSeedBlockout
    {
        public string id = "block-01";
        public string primitive = "Cube";
        public Vector3 position;
        public Vector3 rotationEuler;
        public Vector3 scale = Vector3.one;
        public string materialHint = "default";
    }
}
