using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Meta.Runtime.Saves;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Meta
{
    [DisallowMultipleComponent]
    public sealed class ProgressionBoostApplier : MonoBehaviour
    {
        [SerializeField] private AvatarMotor avatarMotor;
        [SerializeField] private HoverboardMotor hoverboardMotor;
        [SerializeField] private float refreshSeconds = 2f;

        private float nextRefreshAt;

        private void Awake()
        {
            if (avatarMotor == null)
            {
                avatarMotor = GetComponent<AvatarMotor>();
            }

            if (hoverboardMotor == null)
            {
                hoverboardMotor = GetComponentInChildren<HoverboardMotor>(true);
            }
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextRefreshAt)
            {
                return;
            }

            Apply();
        }

        public void Apply()
        {
            var state = PlayerProfileStore.Load();
            if (avatarMotor != null)
            {
                avatarMotor.SpeedMultiplier = state.ResolveWalkSpeedMultiplier();
            }

            if (hoverboardMotor != null)
            {
                hoverboardMotor.SpeedMultiplier = state.ResolveBoardSpeedMultiplier();
            }

            nextRefreshAt = Time.unscaledTime + refreshSeconds;
        }
    }
}
