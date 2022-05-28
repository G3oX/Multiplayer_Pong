using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace networking_2
{
    public class PlayerInputHandler : MonoBehaviour
    {
        float h_input;
        PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            h_input = Input.GetAxis("Horizontal");

            _playerController.SetInputVector(h_input);
        }

        public NetworkInputData GetNetworkInput()
        {
            NetworkInputData data = new NetworkInputData();
            data.direction = h_input;

            return data;
        }
    }

}
