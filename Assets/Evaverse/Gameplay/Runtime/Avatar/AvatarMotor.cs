using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Evaverse.Gameplay.Runtime.Avatar
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class AvatarMotor : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float walkSpeed = 5.5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;

        [Header("Look")]
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float mouseSensitivity = 120f;
        [SerializeField] private float minPitch = -55f;
        [SerializeField] private float maxPitch = 75f;

        private CharacterController controller;
        private float verticalVelocity;
        private float pitch;

        public Vector2 MoveInput { get; private set; }
        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            ReadInput();
            UpdateLook();
            UpdateMovement();
        }

        private void ReadInput()
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 moveInput = Vector2.zero;
            IsSprinting = false;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) { moveInput.y += 1f; }
                if (Keyboard.current.sKey.isPressed) { moveInput.y -= 1f; }
                if (Keyboard.current.dKey.isPressed) { moveInput.x += 1f; }
                if (Keyboard.current.aKey.isPressed) { moveInput.x -= 1f; }
                IsSprinting = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }

            MoveInput = Vector2.ClampMagnitude(moveInput, 1f);
#else
            MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            MoveInput = Vector2.ClampMagnitude(MoveInput, 1f);
            IsSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
        }

        private void UpdateLook()
        {
            float yawDelta = 0f;
            float pitchDelta = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                yawDelta = mouseDelta.x;
                pitchDelta = mouseDelta.y;
            }
#else
            yawDelta = Input.GetAxis("Mouse X") * 10f;
            pitchDelta = Input.GetAxis("Mouse Y") * 10f;
#endif

            transform.Rotate(Vector3.up, yawDelta * mouseSensitivity * Time.deltaTime);

            if (cameraPivot == null)
            {
                return;
            }

            pitch -= pitchDelta * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void UpdateMovement()
        {
            float speed = IsSprinting ? sprintSpeed : walkSpeed;
            Vector3 localMove = new Vector3(MoveInput.x, 0f, MoveInput.y);
            Vector3 worldMove = transform.TransformDirection(localMove) * speed;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#else
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
#endif

            if (controller.isGrounded && jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;
            worldMove.y = verticalVelocity;

            controller.Move(worldMove * Time.deltaTime);

            if (localMove.sqrMagnitude > 0.001f)
            {
                Vector3 desiredForward = new Vector3(worldMove.x, 0f, worldMove.z).normalized;
                transform.forward = Vector3.Lerp(transform.forward, desiredForward, turnSpeed * Time.deltaTime);
            }
        }
    }
}
