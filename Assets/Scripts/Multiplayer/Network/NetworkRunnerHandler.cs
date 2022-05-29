using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Multiplayer
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        public NetworkRunner networkRunnerPrefab;

        NetworkRunner networkRunner;


        // Start is called before the first frame update
        void Start()
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";

            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

            Debug.Log($"Server NetworkRunner started");
        }

        protected virtual Task InitializeNetworkRunner (NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
        {
            var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneObjectProvider>().FirstOrDefault();

            if(sceneObjectProvider == null)
            {
                // Recoje los objetos que existan en la escena y sean networked 
                sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            { 
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestRoom",
                Initialized = initialized,
                SceneObjectProvider = sceneObjectProvider
            });
     
        }

    }

}
