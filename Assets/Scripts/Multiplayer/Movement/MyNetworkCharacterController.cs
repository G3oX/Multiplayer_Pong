using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class MyNetworkCharacterController : NetworkTransform
    {

        [Header("Character Controller Settings")]
        public float acceleration = 10.0f;
        public float braking = 10.0f;
        public float maxSpeed = 2.0f;
        
        [Tooltip("Limita el movimiento del jugador en el eje horizontal")]
        [SerializeField] float horizontalLimits;

        [Networked]
        [HideInInspector]
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Sets the default teleport interpolation velocity to be the CC's current velocity.
        /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
        /// </summary>
        protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

        public virtual void Move(Vector2 direction)
        {
            var deltaTime = Runner.DeltaTime;
            var previousPos = transform.position;
            var moveVelocity = Velocity;
            direction = direction.normalized;

            var horizontalVel = default(Vector2);
            horizontalVel.x = moveVelocity.x;

            if (direction == default)
            {
                horizontalVel = Vector2.Lerp(horizontalVel, default, braking * deltaTime);
            }
            else
            {
                horizontalVel = Vector2.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
            }

            moveVelocity.x = horizontalVel.x;

            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;

            clampHorizontalPosition(moveVelocity);
        }

        private void clampHorizontalPosition(Vector3 moveVelocity)
        {
            if(transform.position.x <= horizontalLimits && transform.position.x >= -horizontalLimits)
            {
                transform.position += moveVelocity;
            }
            else if(transform.position.x > horizontalLimits) { transform.position = new Vector2(horizontalLimits,transform.position.y); }
            else if (transform.position.x < -horizontalLimits) { transform.position = new Vector2(-horizontalLimits, transform.position.y); }

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector2(horizontalLimits, transform.position.y), 0.2f);
            Gizmos.DrawSphere(new Vector2(-horizontalLimits, transform.position.y), 0.2f);
        }
    }

}

