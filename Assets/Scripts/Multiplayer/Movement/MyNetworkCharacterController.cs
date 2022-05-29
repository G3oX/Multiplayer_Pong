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

        [Networked]
        [HideInInspector]
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Sets the default teleport interpolation velocity to be the CC's current velocity.
        /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
        /// </summary>
        protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public virtual void Move(Vector3 direction)
        {
            var deltaTime = Runner.DeltaTime;
            var previousPos = transform.position;
            var moveVelocity = Velocity;

            direction = direction.normalized;

            var horizontalVel = default(Vector3);
            horizontalVel.x = moveVelocity.x;
            horizontalVel.z = moveVelocity.z;

            if (direction == default)
            {
                horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
            }
            else
            {
                horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);
            }

            moveVelocity.x = horizontalVel.x;
            moveVelocity.z = horizontalVel.z;

            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;

            transform.position += moveVelocity;
        }
    }

}

