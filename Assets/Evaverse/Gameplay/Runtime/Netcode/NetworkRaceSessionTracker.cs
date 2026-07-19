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
        private readonly NetworkVariable<float> sessionCountdownSeconds = new(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private float lastSyncedStartRequestAt = -10f;

        public NetworkList<RaceFinishEntry> Finishes => finishes;
        public bool HasActiveSessionCountdown => sessionCountdownSeconds.Value > 0f;

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
                sessionCountdownSeconds.Value = 0f;
            }
        }

        public void RequestSyncedStart(float countdownSeconds)
        {
            if (Time.unscaledTime - lastSyncedStartRequestAt < 1f)
            {
                return;
            }

            lastSyncedStartRequestAt = Time.unscaledTime;

            if (IsServer)
            {
                BeginSyncedStart(countdownSeconds);
                return;
            }

            RequestSyncedStartServerRpc(countdownSeconds);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSyncedStartServerRpc(float countdownSeconds)
        {
            BeginSyncedStart(countdownSeconds);
        }

        private void BeginSyncedStart(float countdownSeconds)
        {
            if (!IsServer)
            {
                return;
            }

            finishes.Clear();
            float duration = Mathf.Max(0.1f, countdownSeconds);
            sessionCountdownSeconds.Value = duration;
            StartCountdownClientRpc(duration);
        }

        [ClientRpc]
        private void StartCountdownClientRpc(float countdownSeconds)
        {
            RaceLapTracker[] trackers = FindObjectsByType<RaceLapTracker>(FindObjectsSortMode.None);
            for (int i = 0; i < trackers.Length; i++)
            {
                RaceLapTracker tracker = trackers[i];
                NetworkObject networkObject = tracker.GetComponent<NetworkObject>();
                if (networkObject == null || !networkObject.IsOwner)
                {
                    continue;
                }

                tracker.ResetProgress();
                tracker.StartCountdown(countdownSeconds);
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
