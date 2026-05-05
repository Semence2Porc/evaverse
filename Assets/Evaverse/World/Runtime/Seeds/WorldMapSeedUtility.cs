using System;
using UnityEngine;

namespace Evaverse.World.Runtime.Seeds
{
    public static class WorldMapSeedUtility
    {
        public static bool TryParse(TextAsset jsonAsset, out WorldMapSeedPayload payload, out string error)
        {
            if (jsonAsset == null)
            {
                payload = null;
                error = "No seed JSON asset has been assigned.";
                return false;
            }

            return TryParse(jsonAsset.text, out payload, out error);
        }

        public static bool TryParse(string json, out WorldMapSeedPayload payload, out string error)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                payload = null;
                error = "The provided map seed JSON is empty.";
                return false;
            }

            try
            {
                payload = JsonUtility.FromJson<WorldMapSeedPayload>(json);
            }
            catch (Exception exception)
            {
                payload = null;
                error = $"Failed to parse map seed JSON: {exception.Message}";
                return false;
            }

            if (payload == null)
            {
                error = "The map seed JSON could not be deserialized.";
                return false;
            }

            if (payload.metadata == null)
            {
                payload.metadata = new WorldMapSeedMetadata();
            }

            payload.spawns ??= Array.Empty<WorldMapSeedSpawn>();
            payload.biomeZones ??= Array.Empty<WorldMapSeedBiomeZone>();
            payload.regions ??= Array.Empty<WorldMapSeedRegion>();
            payload.pointsOfInterest ??= Array.Empty<WorldMapSeedPointOfInterest>();
            payload.blockout ??= Array.Empty<WorldMapSeedBlockout>();

            error = string.Empty;
            return true;
        }
    }
}
