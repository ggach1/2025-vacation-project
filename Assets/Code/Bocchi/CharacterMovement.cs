using Code.System;
using UnityEngine;

namespace Code.Bocchi
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("settings")]
        [SerializeField] float speed = 5f;
        [SerializeField] float sprintSpeed = 8f;
        [SerializeField] float acceleration = 30f;
        [SerializeField] float airAcceleration = 12f;
        [SerializeField] float gravity = -25f;
        [SerializeField] float jumpHeight = 1.2f;
        [SerializeField] float groundedStickForce = -2f;

        [Header("components")]
        [SerializeField] CharacterController controller;
        [SerializeField] InputSO input;
        [SerializeField] Transform moveReference;

        public bool CanManualMovement { get; set; } = true;
        public bool IsGrounded { get; private set; }
        public Vector3 Velocity => _horizontalVelocity + Vector3.up * _verticalVelocity;

        Vector3 _horizontalVelocity;
        float _verticalVelocity;

        private void Reset()
        {
            controller = GetComponentInParent<CharacterController>();
            moveReference = controller != null ? controller.transform : transform.root;
        }

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<CharacterController>();
            }

            if (moveReference == null)
            {
                moveReference = controller != null ? controller.transform : transform.root;
            }

            if (controller != null && controller.TryGetComponent(out Rigidbody body))
            {
                body.isKinematic = true;
                body.useGravity = false;
            }
        }

        private void Update()
        {
            input?.Refresh();

            float deltaTime = Time.deltaTime;
            CalculateMovement(deltaTime);
            Move(deltaTime);
        }

        private void CalculateMovement(float deltaTime)
        {
            Vector2 moveInput = CanManualMovement && input != null ? input.MovementKey : Vector2.zero;
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);

            Transform reference = moveReference != null ? moveReference : transform;
            Vector3 forward = Vector3.ProjectOnPlane(reference.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(reference.right, Vector3.up).normalized;

            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.forward;
            }

            if (right.sqrMagnitude < 0.001f)
            {
                right = Vector3.right;
            }

            float targetSpeed = input != null && input.SprintHeld ? sprintSpeed : speed;
            Vector3 targetVelocity = (right * moveInput.x + forward * moveInput.y) * targetSpeed;
            float currentAcceleration = IsGrounded ? acceleration : airAcceleration;

            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, targetVelocity, currentAcceleration * deltaTime);
        }

        private void Move(float deltaTime)
        {
            if (controller == null)
            {
                return;
            }

            IsGrounded = controller.isGrounded;

            if (IsGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = groundedStickForce;
            }

            bool jumpPressed = input != null && input.ConsumeJumpPressed();

            if (CanManualMovement && IsGrounded && jumpPressed)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                IsGrounded = false;
            }
            else
            {
                _verticalVelocity += gravity * deltaTime;
            }

            Vector3 velocity = _horizontalVelocity + Vector3.up * _verticalVelocity;
            controller.Move(velocity * deltaTime);

            IsGrounded = controller.isGrounded;
        }

        public void StopImmediately()
        {
            _horizontalVelocity = Vector3.zero;
            _verticalVelocity = 0f;
        }
    }
}
