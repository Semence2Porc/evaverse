using System;
using System.Collections.Generic;
using Evaverse.World.Runtime.Definitions;
using UnityEngine;

namespace Evaverse.World.Runtime.Authoring
{
    [DisallowMultipleComponent]
    public sealed class WorldSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string spawnId = "default";
        [SerializeField] private string label = "Default Spawn";
        [SerializeField] private string sceneNameOverride;
        [SerializeField] private bool preferredForFirstTimePlayers = true;
        [SerializeField] private List<string> tags = new();
        [SerializeField] private Color gizmoColor = new(0.1f, 0.85f, 1f, 1f);

        public string SpawnId => spawnId;

        public WorldSpawnDefinitionSnapshot CreateSnapshot()
        {
            string resolvedSceneName = string.IsNullOrWhiteSpace(sceneNameOverride) ? gameObject.scene.name : sceneNameOverride;

            return new WorldSpawnDefinitionSnapshot(
                spawnId,
                label,
                resolvedSceneName,
                transform.position,
                transform.eulerAngles,
                preferredForFirstTimePlayers,
                tags.ToArray());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Vector3 position = transform.position;
            Gizmos.DrawSphere(position, 0.35f);
            Gizmos.DrawLine(position, position + (transform.forward * 2f));
        }
    }

    [Serializable]
    public readonly struct WorldSpawnDefinitionSnapshot
    {
        public WorldSpawnDefinitionSnapshot(
            string spawnId,
            string label,
            string sceneName,
            Vector3 position,
            Vector3 eulerAngles,
            bool preferredForFirstTimePlayers,
            IReadOnlyList<string> tags)
        {
            SpawnId = spawnId;
            Label = label;
            SceneName = sceneName;
            Position = position;
            EulerAngles = eulerAngles;
            PreferredForFirstTimePlayers = preferredForFirstTimePlayers;
            Tags = tags;
        }

        public string SpawnId { get; }
        public string Label { get; }
        public string SceneName { get; }
        public Vector3 Position { get; }
        public Vector3 EulerAngles { get; }
        public bool PreferredForFirstTimePlayers { get; }
        public IReadOnlyList<string> Tags { get; }
    }
}
