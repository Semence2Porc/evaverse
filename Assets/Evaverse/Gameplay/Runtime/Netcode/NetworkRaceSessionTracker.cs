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
        private readonly NetworkVariable<double> countdownEndServerTime = new(-1d);

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
                countdownEndServerTime.Value = -1d;
            }
        }

        public void RequestSynchronizedCountdown(float durationSeconds)
        {
            if (!IsSpawned)
            {
                return;
            }

            if (IsServer)
            {
                BeginSynchronizedCountdown(durationSeconds);
            }
            else
            {
                RequestSynchronizedCountdownServerRpc(durationSeconds);
            }
        }

        public bool TryGetSynchronizedCountdownRemaining(out float remainingSeconds)
        {
            remainingSeconds = 0f;
            if (!IsSpawned || NetworkManager.Singleton == null)
            {
                return false;
            }

            double endTime = countdownEndServerTime.Value;
            if (endTime < 0d)
            {
                return false;
            }

            remainingSeconds = (float)(endTime - NetworkManager.Singleton.ServerTime.Time);
            return remainingSeconds > -0.05f;
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSynchronizedCountdownServerRpc(float durationSeconds)
        {
            BeginSynchronizedCountdown(durationSeconds);
        }

        private void BeginSynchronizedCountdown(float durationSeconds)
        {
            if (!IsServer || NetworkManager.Singleton == null)
            {
                return;
            }

            if (TryGetSynchronizedCountdownRemaining(out float activeRemaining) && activeRemaining > 0.05f)
            {
                return;
            }

            countdownEndServerTime.Value = NetworkManager.Singleton.ServerTime.Time + Mathf.Max(0.1f, durationSeconds);
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
