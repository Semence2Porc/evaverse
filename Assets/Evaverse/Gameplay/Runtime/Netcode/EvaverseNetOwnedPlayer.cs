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
        [SerializeField] private string spawnId = "plaza-default";

        private AvatarMotor avatarMotor;
        private HoverboardMountController mountController;
        private RaceLapTracker lapTracker;
        private RacePrototypeHud raceHud;
        private CinemachinePlayerRig cinemachineRig;
        private Camera playerCamera;
        private AudioListener audioListener;

        public override void OnNetworkSpawn()
        {
            CacheComponents();
            ApplyOwnership(IsOwner);

            if (IsOwner)
            {
                PlaceAtSpawn();
                WireSceneReferences();
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
            cinemachineRig = GetComponent<CinemachinePlayerRig>();
            playerCamera = GetComponentInChildren<Camera>(true);
            audioListener = GetComponentInChildren<AudioListener>(true);
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

        private void WireSceneReferences()
        {
            RaceCourseDefinition course = FindFirstObjectByType<RaceCourseDefinition>();
            if (lapTracker != null && course != null)
            {
                lapTracker.SetCourse(course);
            }

            HoverboardMotor boardMotor = FindFirstObjectByType<HoverboardMotor>();
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
        }
    }
}
