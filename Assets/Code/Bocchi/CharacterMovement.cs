using Code.System;
using System;
using UnityEngine;

namespace Code.Bocchi
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("settings")]
        [SerializeField] float speed = 5f;
        [SerializeField] float gravity = -9.81f;

        [Header("components")]
        [SerializeField] CharacterController controller;
        [SerializeField] InputSO input;

        public bool CanManualMovement { get; set; } = true;

        Vector3 _velocity;
        Vector3 _movementDirection;

        private void FixedUpdate()
        {
            CalculateMovement();
            Move();
        }

        private void CalculateMovement()
        {
            if (!CanManualMovement)
            {
                _movementDirection = Vector3.zero;
                return;
            }

            _movementDirection = input.MovementKey * speed;
        }

        private void Move()
        {
            controller.Move(_movementDirection * Time.fixedDeltaTime);

            if (controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += gravity * Time.fixedDeltaTime;
            }

            controller.Move(_velocity * Time.fixedDeltaTime);
        }

        public void StopImmediately()
        {
            _movementDirection = Vector3.zero;
        }
    }
}