using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;


namespace Multiplayer
{
    public class MySceneManager : MonoBehaviour
    {

        private void Spawned()
        {

        }

        public void loadMenuScene()
        {
            SceneManager.LoadScene(0);
        }
    }
}

