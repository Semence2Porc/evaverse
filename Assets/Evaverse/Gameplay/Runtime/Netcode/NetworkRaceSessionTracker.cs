using System.Collections.Generic;
using Evaverse.Gameplay.Runtime.Racing;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public sealed class NetworkRaceSessionTracker : NetworkBehaviour
    {
        public static NetworkRaceSessionTracker Instance { get; private set; }

        private readonly NetworkList<RaceFinishEntry> finishes = new();

        public NetworkList<RaceFinishEntry> Finishes => finishes;

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                finishes.Clear();
            }
        }

        public void RegisterFinish(ulong clientId, string displayName, float finishSeconds)
        {
            if (!IsServer)
            {
                return;
            }

            for (int i = 0; i < finishes.Count; i++)
            {
                if (finishes[i].ClientId == clientId)
                {
                    return;
                }
            }

            RaceFinishEntry entry = new RaceFinishEntry
            {
                ClientId = clientId,
                Place = finishes.Count + 1,
                FinishSeconds = finishSeconds,
                DisplayName = displayName
            };

            finishes.Add(entry);
        }

        public List<RaceFinishEntry> GetSortedFinishes()
        {
            List<RaceFinishEntry> sorted = new List<RaceFinishEntry>(finishes.Count);
            for (int i = 0; i < finishes.Count; i++)
            {
                sorted.Add(finishes[i]);
            }

            sorted.Sort((left, right) => left.Place.CompareTo(right.Place));
            return sorted;
        }
    }
}
