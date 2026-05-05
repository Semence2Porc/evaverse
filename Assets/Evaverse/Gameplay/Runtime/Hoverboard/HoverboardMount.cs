using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Hoverboard
{
    public sealed class HoverboardMount : MonoBehaviour
    {
        [SerializeField] private Transform riderSocket;
        [SerializeField] private GameObject riderRoot;

        public bool IsMounted { get; private set; }

        public void Mount(Transform rider)
        {
            if (rider == null || riderSocket == null)
            {
                return;
            }

            rider.SetParent(riderSocket, true);
            rider.localPosition = Vector3.zero;
            rider.localRotation = Quaternion.identity;

            if (riderRoot != null)
            {
                riderRoot.SetActive(false);
            }

            IsMounted = true;
        }

        public void Dismount(Transform rider)
        {
            if (rider == null)
            {
                return;
            }

            rider.SetParent(null, true);

            if (riderRoot != null)
            {
                riderRoot.SetActive(true);
            }

            IsMounted = false;
        }
    }
}
