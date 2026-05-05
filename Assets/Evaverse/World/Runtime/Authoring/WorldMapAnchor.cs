using Evaverse.World.Runtime.Definitions;
using UnityEngine;

namespace Evaverse.World.Runtime.Authoring
{
    [DisallowMultipleComponent]
    public sealed class WorldMapAnchor : MonoBehaviour
    {
        [SerializeField] private WorldMapDefinition definition;
        [SerializeField] private string fallbackSpawnId = "default";

        public WorldMapDefinition Definition => definition;
        public string FallbackSpawnId => fallbackSpawnId;

        public Pose ResolveSpawnPose(string preferredSpawnId = null)
        {
            if (definition == null)
            {
                return new Pose(transform.position, transform.rotation);
            }

            string spawnId = string.IsNullOrWhiteSpace(preferredSpawnId) ? fallbackSpawnId : preferredSpawnId;
            return definition.ResolveSpawnPose(spawnId);
        }

        public bool TryResolveRegion(Vector3 worldPosition, out WorldRegionDefinition region)
        {
            if (definition == null)
            {
                region = null;
                return false;
            }

            return definition.TryGetRegion(worldPosition, out region);
        }
    }
}
