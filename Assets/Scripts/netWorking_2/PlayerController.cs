using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

namespace networking_2
{
    public class PlayerController : NetworkBehaviour
    {

        [SerializeField] float acel;
        [SerializeField] float maxSpeed;

        Rigidbody2D _rb2D;
        float _inputVector;

        // Start is called before the first frame update
        void Awake()
        {
            _rb2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame and on Networked
        public override void FixedUpdateNetwork()
        {
            if(GetInput(out NetworkInputData data))
            {
                _inputVector = data.direction;
            }


            if (_rb2D.velocity.x >= maxSpeed)
            {
                _inputVector = 0;
            }

            _rb2D.AddForce(new Vector2(_inputVector * acel * Runner.DeltaTime, 0), ForceMode2D.Force);
            
        }

        public void SetInputVector(float inputVector)
        {
            _inputVector = inputVector;
        }
    }

}
