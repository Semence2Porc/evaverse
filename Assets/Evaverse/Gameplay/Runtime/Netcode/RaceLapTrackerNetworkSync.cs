using Evaverse.Gameplay.Runtime.Racing;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class RaceLapTrackerNetworkSync : NetworkBehaviour
    {
        private readonly NetworkVariable<RaceTrackerSnapshot> snapshot = new(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        private RaceLapTracker tracker;
        private RaceTrackerSnapshot lastPublished;

        public void Configure(RaceLapTracker raceTracker)
        {
            tracker = raceTracker;
        }

        public override void OnNetworkSpawn()
        {
            if (tracker == null)
            {
                tracker = GetComponent<RaceLapTracker>();
            }

            snapshot.OnValueChanged += HandleSnapshotChanged;

            if (!IsOwner)
            {
                tracker?.ApplyNetworkSnapshot(snapshot.Value);
            }
        }

        public override void OnNetworkDespawn()
        {
            snapshot.OnValueChanged -= HandleSnapshotChanged;
        }

        private void Update()
        {
            if (!IsOwner || tracker == null)
            {
                return;
            }

            RaceTrackerSnapshot current = tracker.CreateNetworkSnapshot();
            if (SnapshotsEqual(current, lastPublished))
            {
                return;
            }

            lastPublished = current;
            snapshot.Value = current;
        }

        private void HandleSnapshotChanged(RaceTrackerSnapshot previous, RaceTrackerSnapshot current)
        {
            if (IsOwner || tracker == null)
            {
                return;
            }

            tracker.ApplyNetworkSnapshot(current);
        }

        private static bool SnapshotsEqual(RaceTrackerSnapshot left, RaceTrackerSnapshot right)
        {
            return left.State == right.State
                && left.CurrentLap == right.CurrentLap
                && left.NextCheckpointIndex == right.NextCheckpointIndex
                && Mathf.Approximately(left.CountdownRemaining, right.CountdownRemaining)
                && Mathf.Approximately(left.ElapsedSeconds, right.ElapsedSeconds);
        }
    }
}
