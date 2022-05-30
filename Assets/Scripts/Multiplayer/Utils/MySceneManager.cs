using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;


namespace Multiplayer
{
    public class MySceneManager : SimulationBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void loadMenuScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}

