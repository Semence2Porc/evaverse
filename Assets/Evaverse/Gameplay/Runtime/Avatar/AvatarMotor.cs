using Evaverse.Gameplay.Runtime.Hoverboard;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Mount")]
        [SerializeField] private HoverboardMountController mountController;

        private CharacterController controller;
        private float verticalVelocity;
        private float pitch;

        public Vector2 MoveInput { get; private set; }
        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (mountController == null)
            {
                mountController = GetComponent<HoverboardMountController>();
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            ReadInput();
            UpdateLook();
            UpdateMovement();
        }

        private void ReadInput()
        {
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

            if (Gamepad.current != null)
            {
                moveInput += Gamepad.current.leftStick.ReadValue();
                IsSprinting |= Gamepad.current.leftStickButton.isPressed;
            }

            MoveInput = Vector2.ClampMagnitude(moveInput, 1f);
        }

        private void UpdateLook()
        {
            float yawDegrees = 0f;
            float pitchDegrees = 0f;

            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                float scale = mouseSensitivity * Time.deltaTime;
                yawDegrees += mouseDelta.x * scale;
                pitchDegrees += mouseDelta.y * scale;
            }

            if (Gamepad.current != null)
            {
                Vector2 look = Gamepad.current.rightStick.ReadValue();
                float stickScale = 110f * Time.deltaTime;
                yawDegrees += look.x * stickScale;
                pitchDegrees += look.y * stickScale;
            }

            transform.Rotate(Vector3.up, yawDegrees);

            if (cameraPivot == null)
            {
                return;
            }

            pitch -= pitchDegrees;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void UpdateMovement()
        {
            if (mountController != null && mountController.IsMounted)
            {
                return;
            }

            if (controller == null || !controller.enabled)
            {
                return;
            }

            float speed = IsSprinting ? sprintSpeed : walkSpeed;
            Vector3 localMove = new Vector3(MoveInput.x, 0f, MoveInput.y);
            Vector3 worldMove = transform.TransformDirection(localMove) * speed;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            if (!jumpPressed && Gamepad.current != null)
            {
                jumpPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;
            }

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
