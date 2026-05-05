using Unity.Cinemachine;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.View
{
    /// <summary>
    /// Wires a <see cref="CinemachineBrain"/> on the real camera and a single follow virtual camera
    /// that tracks the local pitch pivot (yaw stays on the avatar root).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CinemachinePlayerRig : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraPivot;

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

            if (playerCamera.GetComponent<CinemachineBrain>() == null)
            {
                playerCamera.gameObject.AddComponent<CinemachineBrain>();
            }

            GameObject vcamObject = new GameObject("CM Follow Camera");
            vcamObject.transform.SetParent(transform, false);

            CinemachineCamera vcam = vcamObject.AddComponent<CinemachineCamera>();
            vcam.Target = new CameraTarget
            {
                TrackingTarget = cameraPivot,
                CustomLookAtTarget = false
            };

            vcam.Lens.FieldOfView = playerCamera.fieldOfView > 1f ? playerCamera.fieldOfView : 58f;
            vcam.Lens.NearClipPlane = playerCamera.nearClipPlane;
            vcam.Lens.FarClipPlane = playerCamera.farClipPlane;
            vcam.Priority.Value = 10;

            CinemachineFollow follow = vcamObject.AddComponent<CinemachineFollow>();
            follow.FollowOffset = new Vector3(0f, 0.35f, -7.5f);
            follow.TrackerSettings.BindingMode = BindingMode.LockToTargetNoRoll;
            follow.TrackerSettings.PositionDamping = Vector3.zero;
            follow.TrackerSettings.RotationDamping = Vector3.zero;

            CinemachineRotateWithFollowTarget aim = vcamObject.AddComponent<CinemachineRotateWithFollowTarget>();
            aim.Damping = 0f;
        }
    }
}
