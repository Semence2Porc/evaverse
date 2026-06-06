using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Meta.Runtime.Saves;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class RaceFinishReporter : NetworkBehaviour
    {
        [SerializeField] private int ticketReward = 15;

        private RaceLapTracker tracker;
        private NetworkPlayerDisplayName displayName;
        private bool finishReported;

        public void Configure(RaceLapTracker raceTracker, NetworkPlayerDisplayName playerDisplayName)
        {
            tracker = raceTracker;
            displayName = playerDisplayName;
        }

        private void Update()
        {
            if (!IsOwner || tracker == null)
            {
                return;
            }

            if (!tracker.Finished)
            {
                finishReported = false;
                return;
            }

            if (finishReported)
            {
                return;
            }

            finishReported = true;
            string name = displayName != null ? displayName.DisplayName : $"Racer {OwnerClientId}";
            ReportFinishServerRpc(new FixedString32Bytes(name), tracker.ElapsedSeconds);
            AwardLocalProgression();
        }

        [ServerRpc]
        private void ReportFinishServerRpc(FixedString32Bytes playerName, float finishSeconds, ServerRpcParams rpcParams = default)
        {
            NetworkRaceSessionTracker sessionTracker = NetworkRaceSessionTracker.Instance;
            if (sessionTracker == null)
            {
                return;
            }

            sessionTracker.RegisterFinish(rpcParams.Receive.SenderClientId, playerName.ToString(), finishSeconds);
        }

        private void AwardLocalProgression()
        {
            var state = PlayerProfileStore.Load();
            state.CompleteRace(ticketReward);
            PlayerProfileStore.Save(state);
        }
    }
}
