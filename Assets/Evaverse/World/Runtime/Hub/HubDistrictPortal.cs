using System;
using UnityEngine;

namespace Evaverse.World.Runtime.Hub
{
    /// <summary>
    /// Trigger volume for district portal pads in the Snowbound Nexus hub.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class HubDistrictPortal : MonoBehaviour
    {
        public static event Action<HubDistrictPortal, bool> ProximityChanged;

        [SerializeField] private string districtName = "District";
        [SerializeField] private string promptText = "Portal online — district travel coming soon.";

        public string DistrictName => districtName;
        public string PromptText => promptText;

        private void Reset()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayerCollider(other))
            {
                return;
            }

            ProximityChanged?.Invoke(this, true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayerCollider(other))
            {
                return;
            }

            ProximityChanged?.Invoke(this, false);
        }

        private static bool IsPlayerCollider(Collider other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.CompareTag("Player"))
            {
                return true;
            }

            return other.GetComponentInParent<CharacterController>() != null;
        }
    }
}
