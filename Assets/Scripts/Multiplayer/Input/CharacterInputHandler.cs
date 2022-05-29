using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer
{
    public class CharacterInputHandler : MonoBehaviour
    {

        [SerializeField] string horizontalInput = "Horizontal";
        float h_input = 0f;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Movimiento
            h_input = Input.GetAxis(horizontalInput);
        }

        public NetworkInputData GetNetworkInput()
        {
            NetworkInputData inputData = new NetworkInputData();

            inputData.h_input = h_input;

            return inputData;
        }


    }
}


