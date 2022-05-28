using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace networking_2
{
    public class NetWorkRunnerHandler : MonoBehaviour
    {

        NetworkRunner networkRunner;

        private void Awake()
        {
            networkRunner = GetComponent<NetworkRunner>();
        }


        // Start is called before the first frame update
        void Start()
        {
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        }

        protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
        {
            var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneObjectProvider>().FirstOrDefault();

            if(sceneObjectProvider == null)
            {
                sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                SessionName = "TestSession",
                Initialized = initialized,
                SceneObjectProvider = sceneObjectProvider

            });

        }
    }

}
