using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class CharacterMovementHandler : NetworkBehaviour
    {

        //NetworkCharacterControllerPrototypeEdited _networkCharacterController;
        MyNetworkCharacterController _networkCharacterController;


        private void Awake()
        {
            //_networkCharacterController = GetComponent<NetworkCharacterControllerPrototypeEdited>();
            _networkCharacterController = GetComponent<MyNetworkCharacterController>();
        }

        // NETWORK
        public override void FixedUpdateNetwork()
        {
            if ( GetInput(out NetworkInputData data))
            {
                //Movimiento
                float h_movement = data.h_input;

                _networkCharacterController.Move(new Vector2 (h_movement, 0) * Runner.DeltaTime);
            }
        }

    }
}

