using Evaverse.World.Runtime.Authoring;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Evaverse.Gameplay.Runtime.Hub
{
    [DisallowMultipleComponent]
    public sealed class DistrictPortal : MonoBehaviour
    {
        [SerializeField] private string destinationSpawnId = "plaza-default";
        [SerializeField] private string portalLabel = "District Portal";
        [SerializeField] private float interactDistance = 7f;

        public string DestinationSpawnId => destinationSpawnId;
        public string PortalLabel => portalLabel;

        private void Update()
        {
            if (!WasInteractPressed())
            {
                return;
            }

            Transform traveler = FindNearbyLocalTraveler();
            if (traveler == null)
            {
                return;
            }

            Teleport(traveler);
        }

        public void Configure(string spawnId, string label)
        {
            destinationSpawnId = spawnId;
            portalLabel = label;
        }

        private Transform FindNearbyLocalTraveler()
        {
            CharacterController[] controllers = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            Transform best = null;
            float bestDistance = interactDistance;

            for (int i = 0; i < controllers.Length; i++)
            {
                CharacterController controller = controllers[i];
                if (controller == null || !controller.enabled)
                {
                    continue;
                }

                NetworkObject networkObject = controller.GetComponent<NetworkObject>();
                if (networkObject != null && !networkObject.IsOwner)
                {
                    continue;
                }

                float distance = Vector3.Distance(controller.transform.position, transform.position);
                if (distance > bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                best = controller.transform;
            }

            return best;
        }

        private void Teleport(Transform traveler)
        {
            WorldMapAnchor anchor = FindFirstObjectByType<WorldMapAnchor>();
            Pose pose = anchor != null
                ? anchor.ResolveSpawnPose(destinationSpawnId)
                : new Pose(transform.position, transform.rotation);

            CharacterController controller = traveler.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            traveler.SetPositionAndRotation(pose.position, pose.rotation);

            if (controller != null)
            {
                controller.enabled = true;
            }
        }

        private static bool WasInteractPressed()
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                return true;
            }

            return Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, interactDistance * 0.35f);
        }
    }
}
