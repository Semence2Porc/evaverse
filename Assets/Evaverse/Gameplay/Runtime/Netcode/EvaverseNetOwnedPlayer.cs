using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Gameplay.Runtime.Racing;
using Evaverse.Gameplay.Runtime.View;
using Evaverse.World.Runtime.Authoring;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public sealed class EvaverseNetOwnedPlayer : NetworkBehaviour
    {
        private const string PersonalBoardName = "Personal Hoverboard";

        [SerializeField] private string spawnId = "plaza-default";

        private AvatarMotor avatarMotor;
        private HoverboardMountController mountController;
        private HoverboardMotor boardMotor;
        private RaceLapTracker lapTracker;
        private RacePrototypeHud raceHud;
        private RaceCheckpointGuide checkpointGuide;
        private NetworkRaceProgressLabel raceProgressLabel;
        private NetworkPlayerDisplayName displayName;
        private RaceFinishReporter finishReporter;
        private NetworkPlayerMountSync mountSync;
        private RaceLapTrackerNetworkSync raceSync;
        private CinemachinePlayerRig cinemachineRig;
        private Camera playerCamera;
        private AudioListener audioListener;

        public override void OnNetworkSpawn()
        {
            CacheComponents();
            ApplyOwnership(IsOwner);

            WirePersonalBoard();

            if (IsOwner)
            {
                PlaceAtSpawn();
            }
        }

        public override void OnNetworkDespawn()
        {
            ApplyOwnership(false);
        }

        private void CacheComponents()
        {
            avatarMotor = GetComponent<AvatarMotor>();
            mountController = GetComponent<HoverboardMountController>();
            lapTracker = GetComponent<RaceLapTracker>();
            raceHud = GetComponent<RacePrototypeHud>();
            checkpointGuide = GetComponent<RaceCheckpointGuide>();
            raceProgressLabel = GetComponent<NetworkRaceProgressLabel>();
            displayName = GetComponent<NetworkPlayerDisplayName>();
            finishReporter = GetComponent<RaceFinishReporter>();
            mountSync = GetComponent<NetworkPlayerMountSync>();
            raceSync = GetComponent<RaceLapTrackerNetworkSync>();
            cinemachineRig = GetComponent<CinemachinePlayerRig>();
            playerCamera = GetComponentInChildren<Camera>(true);
            audioListener = GetComponentInChildren<AudioListener>(true);
            boardMotor = transform.Find(PersonalBoardName)?.GetComponent<HoverboardMotor>();
        }

        private void ApplyOwnership(bool isOwner)
        {
            if (avatarMotor != null)
            {
                avatarMotor.InputEnabled = isOwner;
            }

            if (mountController != null)
            {
                mountController.InputEnabled = isOwner;
            }

            if (boardMotor != null)
            {
                boardMotor.InputEnabled = isOwner;
            }

            if (raceHud != null)
            {
                raceHud.enabled = isOwner;
            }

            if (cinemachineRig != null)
            {
                cinemachineRig.enabled = isOwner;
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = isOwner;
            }

            if (audioListener != null)
            {
                audioListener.enabled = isOwner;
            }
        }

        private void PlaceAtSpawn()
        {
            WorldMapAnchor anchor = FindFirstObjectByType<WorldMapAnchor>();
            Pose pose = anchor != null
                ? anchor.ResolveSpawnPose(spawnId)
                : new Pose(transform.position, transform.rotation);

            float spread = (OwnerClientId % 8) * 1.5f;
            Vector3 offset = pose.rotation * new Vector3(spread, 0f, 0f);
            transform.SetPositionAndRotation(pose.position + offset, pose.rotation);
        }

        private void WirePersonalBoard()
        {
            RaceCourseDefinition course = FindFirstObjectByType<RaceCourseDefinition>();
            if (lapTracker != null && course != null)
            {
                lapTracker.SetCourse(course);
            }

            if (boardMotor == null)
            {
                return;
            }

            HoverboardMount boardMount = boardMotor.GetComponent<HoverboardMount>();
            Transform dismountPoint = boardMotor.transform.Find("Dismount Point");

            if (mountController != null)
            {
                mountController.Configure(
                    transform,
                    GetComponent<CharacterController>(),
                    boardMotor,
                    boardMount,
                    dismountPoint);
            }

            if (raceHud != null)
            {
                raceHud.Configure(lapTracker, boardMotor);
            }

            if (checkpointGuide != null)
            {
                Transform arrow = transform.Find("Checkpoint Arrow");
                checkpointGuide.Configure(lapTracker, arrow);
            }

            if (mountSync != null)
            {
                mountSync.Configure(mountController, boardMotor, transform);
            }

            if (raceProgressLabel != null)
            {
                TextMesh label = transform.Find("Race Progress Label")?.GetComponent<TextMesh>();
                raceProgressLabel.Configure(lapTracker, label);
            }

            if (raceSync != null)
            {
                raceSync.Configure(lapTracker);
            }

            if (displayName != null)
            {
                TextMesh nameLabel = transform.Find("Player Name Label")?.GetComponent<TextMesh>();
                displayName.Configure(nameLabel);
            }

            if (finishReporter != null)
            {
                finishReporter.Configure(lapTracker, displayName);
            }
        }
    }
}
