using Evaverse.Gameplay.Runtime.Avatar;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Evaverse.Gameplay.Runtime.Hoverboard
{
    public sealed class HoverboardMountController : MonoBehaviour
    {
        [SerializeField] private AvatarMotor avatarMotor;
        [SerializeField] private CharacterController avatarController;
        [SerializeField] private HoverboardMotor hoverboardMotor;
        [SerializeField] private HoverboardMount hoverboardMount;
        [SerializeField] private Transform rider;
        [SerializeField] private Transform dismountPoint;
        [SerializeField] private float mountDistance = 4f;

        public bool IsMounted => hoverboardMount != null && hoverboardMount.IsMounted;

        private void Awake()
        {
            if (avatarMotor == null)
            {
                avatarMotor = GetComponent<AvatarMotor>();
            }

            if (avatarController == null)
            {
                avatarController = GetComponent<CharacterController>();
            }

            if (rider == null)
            {
                rider = transform;
            }

            if (hoverboardMotor != null)
            {
                hoverboardMotor.enabled = IsMounted;
            }
        }

        private void Update()
        {
            if (!WasMountTogglePressed())
            {
                return;
            }

            if (IsMounted)
            {
                Dismount();
            }
            else
            {
                TryMount();
            }
        }

        private bool WasMountTogglePressed()
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                return true;
            }

            return Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;
        }

        private void TryMount()
        {
            if (hoverboardMount == null || hoverboardMotor == null || rider == null)
            {
                return;
            }

            float distance = Vector3.Distance(rider.position, hoverboardMount.transform.position);
            if (distance > mountDistance)
            {
                return;
            }

            if (avatarMotor != null)
            {
                avatarMotor.enabled = false;
            }

            if (avatarController != null)
            {
                avatarController.enabled = false;
            }

            hoverboardMount.Mount(rider);
            hoverboardMotor.enabled = true;
        }

        private void Dismount()
        {
            if (hoverboardMount == null || hoverboardMotor == null || rider == null)
            {
                return;
            }

            hoverboardMotor.enabled = false;
            ResetBoardVelocity(hoverboardMotor);
            hoverboardMount.Dismount(rider);

            Transform exit = dismountPoint != null ? dismountPoint : hoverboardMount.transform;
            rider.SetPositionAndRotation(exit.position, Quaternion.Euler(0f, exit.eulerAngles.y, 0f));

            if (avatarController != null)
            {
                avatarController.enabled = true;
            }

            if (avatarMotor != null)
            {
                avatarMotor.enabled = true;
            }
        }

        private static void ResetBoardVelocity(Component boardComponent)
        {
            if (boardComponent == null || !boardComponent.TryGetComponent(out Rigidbody body))
            {
                return;
            }

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }
}
