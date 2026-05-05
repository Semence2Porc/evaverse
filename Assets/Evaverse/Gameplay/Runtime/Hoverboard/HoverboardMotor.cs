using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Evaverse.Gameplay.Runtime.Hoverboard
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class HoverboardMotor : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float acceleration = 22f;
        [SerializeField] private float maxSpeed = 24f;
        [SerializeField] private float steeringTorque = 9f;
        [SerializeField] private float driftMultiplier = 1.35f;
        [SerializeField] private float boostForce = 12f;

        [Header("Grounding")]
        [SerializeField] private Transform groundProbe;
        [SerializeField] private float groundProbeLength = 2f;
        [SerializeField] private float hoverHeight = 1f;
        [SerializeField] private float hoverStrength = 40f;
        [SerializeField] private LayerMask groundMask = ~0;

        private Rigidbody body;
        private Vector2 steerInput;
        private bool isDrifting;
        private bool boostRequested;

        public float CurrentSpeed => Vector3.Dot(body.linearVelocity, transform.forward);

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            ApplyHover();
            ApplyDrive();
            ApplySteering();
        }

        private void ReadInput()
        {
#if ENABLE_INPUT_SYSTEM
            steerInput = Vector2.zero;
            isDrifting = false;
            boostRequested = false;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) { steerInput.y += 1f; }
                if (Keyboard.current.sKey.isPressed) { steerInput.y -= 1f; }
                if (Keyboard.current.dKey.isPressed) { steerInput.x += 1f; }
                if (Keyboard.current.aKey.isPressed) { steerInput.x -= 1f; }

                isDrifting = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
                boostRequested = Keyboard.current.spaceKey.wasPressedThisFrame;
            }

            steerInput = Vector2.ClampMagnitude(steerInput, 1f);
#else
            steerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            steerInput = Vector2.ClampMagnitude(steerInput, 1f);
            isDrifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            boostRequested = Input.GetKeyDown(KeyCode.Space);
#endif
        }

        private void ApplyHover()
        {
            Transform probe = groundProbe == null ? transform : groundProbe;

            if (!Physics.Raycast(probe.position, Vector3.down, out var hit, groundProbeLength, groundMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            float offset = hoverHeight - hit.distance;
            float lift = offset * hoverStrength;
            body.AddForce(Vector3.up * lift, ForceMode.Acceleration);
        }

        private void ApplyDrive()
        {
            if (Mathf.Abs(CurrentSpeed) < maxSpeed || Mathf.Sign(steerInput.y) != Mathf.Sign(CurrentSpeed))
            {
                body.AddForce(transform.forward * (steerInput.y * acceleration), ForceMode.Acceleration);
            }

            if (boostRequested)
            {
                body.AddForce(transform.forward * boostForce, ForceMode.VelocityChange);
            }

            Vector3 planarVelocity = Vector3.ProjectOnPlane(body.linearVelocity, Vector3.up);
            if (planarVelocity.magnitude > maxSpeed)
            {
                Vector3 clamped = planarVelocity.normalized * maxSpeed;
                body.linearVelocity = new Vector3(clamped.x, body.linearVelocity.y, clamped.z);
            }
        }

        private void ApplySteering()
        {
            float steeringScale = isDrifting ? driftMultiplier : 1f;
            body.AddTorque(Vector3.up * (steerInput.x * steeringTorque * steeringScale), ForceMode.Acceleration);
        }
    }
}
