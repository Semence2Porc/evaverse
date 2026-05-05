using System;
using System.Collections.Generic;
using Evaverse.World.Runtime.Definitions;
using UnityEngine;

namespace Evaverse.World.Runtime.Authoring
{
    [DisallowMultipleComponent]
    public sealed class WorldMapRegistry : MonoBehaviour
    {
        [SerializeField] private WorldMapDefinition defaultMap;
        [SerializeField] private List<WorldMapDefinition> maps = new();

        private readonly Dictionary<string, WorldMapDefinition> lookup = new(StringComparer.OrdinalIgnoreCase);

        public WorldMapDefinition DefaultMap => defaultMap != null ? defaultMap : maps.Count > 0 ? maps[0] : null;
        public IReadOnlyList<WorldMapDefinition> Maps => maps;

        private void Awake()
        {
            RebuildLookup();
        }

        private void OnValidate()
        {
            RebuildLookup();
        }

        public bool TryGetMap(string mapId, out WorldMapDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(mapId))
            {
                definition = DefaultMap;
                return definition != null;
            }

            if (lookup.Count == 0)
            {
                RebuildLookup();
            }

            return lookup.TryGetValue(mapId, out definition);
        }

        private void RebuildLookup()
        {
            lookup.Clear();

            if (defaultMap != null && !string.IsNullOrWhiteSpace(defaultMap.MapId))
            {
                lookup[defaultMap.MapId] = defaultMap;
            }

            for (int i = 0; i < maps.Count; i++)
            {
                WorldMapDefinition map = maps[i];
                if (map == null || string.IsNullOrWhiteSpace(map.MapId))
                {
                    continue;
                }

                lookup[map.MapId] = map;
            }
        }
    }
}
