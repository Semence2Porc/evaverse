using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Racing;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    /// <summary>
    /// Enables local locomotion / cameras only for the owning client. Remote peers become view-only ghosts.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class EvaverseNetOwnedPlayer : NetworkBehaviour
    {
        [SerializeField] private AvatarMotor avatarMotor;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private RaceLapTracker lapTracker;
        [SerializeField] private RacePrototypeHud raceHud;
        [SerializeField] private string spawnObjectName = "spawn-plaza-default";
        [SerializeField] private float spawnSpreadRadius = 3.5f;

        private Camera[] cameras;
        private bool spawnApplied;

        private void Awake()
        {
            ResolveReferences();
        }

        public override void OnNetworkSpawn()
        {
            ResolveReferences();
            ResolveRaceCourse();
            cameras = GetComponentsInChildren<Camera>(true);
            ApplyOwnership();
            ApplyOwnerSpawnPose();
        }

        private void LateUpdate()
        {
            if (!IsSpawned)
            {
                return;
            }

            ApplyOwnership();
        }

        private void ResolveReferences()
        {
            if (avatarMotor == null)
            {
                avatarMotor = GetComponent<AvatarMotor>();
            }

            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (lapTracker == null)
            {
                lapTracker = GetComponent<RaceLapTracker>();
            }

            if (raceHud == null)
            {
                raceHud = GetComponentInChildren<RacePrototypeHud>(true);
            }
        }

        private void ApplyOwnership()
        {
            bool owner = IsOwner;
            if (avatarMotor != null)
            {
                avatarMotor.enabled = owner;
            }

            if (characterController != null)
            {
                characterController.enabled = owner;
            }

            if (lapTracker != null)
            {
                lapTracker.enabled = owner;
            }

            if (raceHud != null)
            {
                raceHud.enabled = owner;
            }

            if (cameras != null)
            {
                for (int i = 0; i < cameras.Length; i++)
                {
                    if (cameras[i] != null)
                    {
                        cameras[i].enabled = owner;
                    }
                }
            }
        }

        private void ApplyOwnerSpawnPose()
        {
            if (!IsOwner || spawnApplied)
            {
                return;
            }

            spawnApplied = true;

            GameObject spawn = GameObject.Find(spawnObjectName);
            if (spawn == null)
            {
                return;
            }

            bool controllerWasEnabled = characterController != null && characterController.enabled;
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            Vector3 offset = ResolveSpawnOffset();
            transform.SetPositionAndRotation(spawn.transform.position + offset, spawn.transform.rotation);

            if (characterController != null)
            {
                characterController.enabled = controllerWasEnabled;
            }
        }

        private Vector3 ResolveSpawnOffset()
        {
            if (OwnerClientId == 0 || spawnSpreadRadius <= 0f)
            {
                return Vector3.zero;
            }

            float angle = (OwnerClientId % 8) * 45f;
            return Quaternion.Euler(0f, angle, 0f) * (Vector3.right * spawnSpreadRadius);
        }

        private void ResolveRaceCourse()
        {
            if (lapTracker == null || lapTracker.Course != null)
            {
                return;
            }

            RaceCourseDefinition course = Object.FindFirstObjectByType<RaceCourseDefinition>();
            if (course != null)
            {
                lapTracker.SetCourse(course);
            }
        }
    }
}
