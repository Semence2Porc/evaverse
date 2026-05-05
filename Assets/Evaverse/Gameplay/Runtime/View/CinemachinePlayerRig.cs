using Evaverse.Gameplay.Runtime.Hoverboard;
using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.View
{
    /// <summary>
    /// Drives the real camera via <see cref="CinemachineBrain"/> and blends between on-foot and
    /// hoverboard follow profiles (priority swap).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CinemachinePlayerRig : MonoBehaviour
    {
        private const int PriorityLive = 10;
        private const int PriorityStandby = 5;

        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private HoverboardMountController mountController;

        private CinemachineCamera vcamOnFoot;
        private CinemachineCamera vcamMounted;

        private void Reset()
        {
            playerCamera = GetComponentInChildren<Camera>(true);
        }

        private void Awake()
        {
            if (cameraPivot == null)
            {
                Debug.LogWarning($"{nameof(CinemachinePlayerRig)}: assign a camera pivot transform.", this);
                return;
            }

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }

            if (playerCamera == null)
            {
                Debug.LogWarning($"{nameof(CinemachinePlayerRig)}: no camera found under {name}.", this);
                return;
            }

            if (mountController == null)
            {
                mountController = GetComponent<HoverboardMountController>();
            }

            if (playerCamera.GetComponent<CinemachineBrain>() == null)
            {
                playerCamera.gameObject.AddComponent<CinemachineBrain>();
            }

            float baseFov = playerCamera.fieldOfView > 1f ? playerCamera.fieldOfView : 58f;
            float nearClip = playerCamera.nearClipPlane;
            float farClip = playerCamera.farClipPlane;

            vcamOnFoot = CreateFollowVcam(
                "CM On Foot",
                new Vector3(0f, 0.35f, -7.5f),
                baseFov,
                nearClip,
                farClip,
                new Vector3(0.18f, 0.18f, 0.18f));

            vcamMounted = CreateFollowVcam(
                "CM Hoverboard",
                new Vector3(0f, 0.55f, -10.25f),
                Mathf.Min(baseFov + 12f, 85f),
                nearClip,
                farClip,
                Vector3.zero);

            ApplyPriorities(false);
        }

        private void LateUpdate()
        {
            if (vcamOnFoot == null || vcamMounted == null)
            {
                return;
            }

            bool mounted = mountController != null && mountController.IsMounted;
            ApplyPriorities(mounted);
        }

        private void ApplyPriorities(bool mounted)
        {
            vcamOnFoot.Priority.Value = mounted ? PriorityStandby : PriorityLive;
            vcamMounted.Priority.Value = mounted ? PriorityLive : PriorityStandby;
        }

        private CinemachineCamera CreateFollowVcam(
            string objectName,
            Vector3 followOffset,
            float fieldOfView,
            float nearClip,
            float farClip,
            Vector3 positionDamping)
        {
            GameObject vcamObject = new GameObject(objectName);
            vcamObject.transform.SetParent(transform, false);

            CinemachineCamera vcam = vcamObject.AddComponent<CinemachineCamera>();
            vcam.Target = new CameraTarget
            {
                TrackingTarget = cameraPivot,
                CustomLookAtTarget = false
            };

            vcam.Lens.FieldOfView = fieldOfView;
            vcam.Lens.NearClipPlane = nearClip;
            vcam.Lens.FarClipPlane = farClip;
            vcam.Priority.Value = PriorityStandby;

            CinemachineFollow follow = vcamObject.AddComponent<CinemachineFollow>();
            follow.FollowOffset = followOffset;
            follow.TrackerSettings.BindingMode = BindingMode.LockToTargetNoRoll;
            follow.TrackerSettings.PositionDamping = positionDamping;
            follow.TrackerSettings.RotationDamping = Vector3.zero;

            CinemachineRotateWithFollowTarget aim = vcamObject.AddComponent<CinemachineRotateWithFollowTarget>();
            aim.Damping = 0f;

            return vcam;
        }
    }
}
