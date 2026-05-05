using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Racing
{
    [RequireComponent(typeof(Collider))]
    public sealed class RaceStartGate : MonoBehaviour
    {
        [SerializeField] private float countdownSeconds = 3f;

        private void Reset()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            RaceLapTracker tracker = other.GetComponentInParent<RaceLapTracker>();
            if (tracker == null)
            {
                return;
            }

            tracker.StartCountdown(countdownSeconds);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2f);
        }
    }
}
