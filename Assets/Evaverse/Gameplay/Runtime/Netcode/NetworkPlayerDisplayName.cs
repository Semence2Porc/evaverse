using Evaverse.Meta.Runtime.Identity;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Netcode
{
    [DisallowMultipleComponent]
    public sealed class NetworkPlayerDisplayName : NetworkBehaviour
    {
        private readonly NetworkVariable<FixedString32Bytes> displayName = new(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        [SerializeField] private float labelHeight = 2.15f;

        private TextMesh nameLabel;
        private Camera viewCamera;

        public string DisplayName => displayName.Value.ToString();

        public void Configure(TextMesh label)
        {
            nameLabel = label;
        }

        public void SetDisplayName(string preferredName)
        {
            if (!IsOwner)
            {
                return;
            }

            string resolved = PlayerDisplayNameStore.Sanitize(preferredName);
            if (string.IsNullOrWhiteSpace(resolved))
            {
                resolved = $"Racer {OwnerClientId}";
            }

            PlayerDisplayNameStore.Save(resolved);
            displayName.Value = new FixedString32Bytes(resolved);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                string preferred = PlayerDisplayNameStore.ResolveOrDefault(OwnerClientId);
                displayName.Value = new FixedString32Bytes(preferred);
            }

            displayName.OnValueChanged += HandleNameChanged;
            ApplyLabel(displayName.Value.ToString());
        }

        public override void OnNetworkDespawn()
        {
            displayName.OnValueChanged -= HandleNameChanged;
        }

        private void HandleNameChanged(FixedString32Bytes previous, FixedString32Bytes current)
        {
            ApplyLabel(current.ToString());
        }

        private void LateUpdate()
        {
            if (nameLabel == null)
            {
                return;
            }

            if (IsOwner)
            {
                nameLabel.gameObject.SetActive(false);
                return;
            }

            nameLabel.gameObject.SetActive(true);
            nameLabel.transform.position = transform.position + Vector3.up * labelHeight;
            FaceCamera(nameLabel.transform);
        }

        private void ApplyLabel(string value)
        {
            if (nameLabel == null)
            {
                return;
            }

            nameLabel.text = value;
        }

        private void FaceCamera(Transform target)
        {
            if (viewCamera == null)
            {
                viewCamera = Camera.main;
            }

            if (viewCamera == null)
            {
                return;
            }

            Vector3 forward = target.position - viewCamera.transform.position;
            if (forward.sqrMagnitude < 0.001f)
            {
                return;
            }

            target.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
    }
}
