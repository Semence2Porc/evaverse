using Evaverse.Gameplay.Runtime.Hoverboard;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class NetworkPlayerMountSync : NetworkBehaviour
    {
        private readonly NetworkVariable<bool> mounted = new(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<Vector3> boardPosition = new(
            Vector3.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<float> boardYaw = new(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        [SerializeField] private Vector3 parkedOffset = new(2.5f, 0f, -1.2f);

        private HoverboardMountController mountController;
        private HoverboardMount boardMount;
        private HoverboardMotor boardMotor;
        private Transform boardRoot;
        private Transform riderTransform;

        public void Configure(HoverboardMountController controller, HoverboardMotor motor, Transform rider)
        {
            mountController = controller;
            boardMotor = motor;
            boardRoot = motor != null ? motor.transform : null;
            boardMount = motor != null ? motor.GetComponent<HoverboardMount>() : null;
            riderTransform = rider;
        }

        public override void OnNetworkSpawn()
        {
            mounted.OnValueChanged += HandleMountedChanged;

            if (IsOwner && mountController != null)
            {
                mountController.MountStateChanged += HandleLocalMountChanged;
            }

            if (boardRoot != null)
            {
                ApplyBoardPose(boardPosition.Value, boardYaw.Value);
                HandleMountedChanged(false, mounted.Value);
            }
        }

        public override void OnNetworkDespawn()
        {
            mounted.OnValueChanged -= HandleMountedChanged;

            if (mountController != null)
            {
                mountController.MountStateChanged -= HandleLocalMountChanged;
            }
        }

        private void HandleLocalMountChanged(bool isMounted)
        {
            if (!IsOwner)
            {
                return;
            }

            mounted.Value = isMounted;
        }

        private void HandleMountedChanged(bool previous, bool current)
        {
            if (IsOwner || mountController == null)
            {
                return;
            }

            mountController.ApplyRemoteMountState(current);
        }

        private void FixedUpdate()
        {
            if (boardRoot == null)
            {
                return;
            }

            if (IsOwner)
            {
                if (!mounted.Value)
                {
                    ParkBoardNearPlayer();
                }

                boardPosition.Value = boardRoot.position;
                boardYaw.Value = boardRoot.eulerAngles.y;
                return;
            }

            ApplyBoardPose(boardPosition.Value, boardYaw.Value);
        }

        private void LateUpdate()
        {
            if (IsOwner || riderTransform == null || boardMount == null || !mounted.Value)
            {
                return;
            }

            Transform socket = boardMount.RiderSocket;
            if (socket == null)
            {
                return;
            }

            riderTransform.SetPositionAndRotation(socket.position, socket.rotation);
        }

        private void ParkBoardNearPlayer()
        {
            Vector3 offset = transform.right * parkedOffset.x + Vector3.up * parkedOffset.y + transform.forward * parkedOffset.z;
            boardRoot.SetPositionAndRotation(
                transform.position + offset,
                Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        }

        private void ApplyBoardPose(Vector3 position, float yaw)
        {
            boardRoot.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));
        }
    }
}
