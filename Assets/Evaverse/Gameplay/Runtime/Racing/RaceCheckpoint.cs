using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Racing
{
    [RequireComponent(typeof(Collider))]
    public sealed class RaceCheckpoint : MonoBehaviour
    {
        [SerializeField] private int index;

        public int Index => index;

        private void OnTriggerEnter(Collider other)
        {
            RaceLapTracker tracker = other.GetComponentInParent<RaceLapTracker>();
            if (tracker == null)
            {
                return;
            }

            tracker.TryPassCheckpoint(this);
        }

        private void Reset()
        {
            var trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2f);
        }
    }
}
